using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Queries.GetSequenceAndSections
{
    public class GetSequenceAndSectionsHandler : IRequestHandler<GetSequenceAndSectionsQuery, GetSequenceAndSectionsResponse>
    {
        private readonly IQnaApiClient _qnaApiClient;

        public GetSequenceAndSectionsHandler(IQnaApiClient qnaApiClient)
        {
            _qnaApiClient = qnaApiClient;
        }

        public async Task<GetSequenceAndSectionsResponse> Handle(GetSequenceAndSectionsQuery request, CancellationToken cancellationToken)
        {
            var sequenceTask = _qnaApiClient.GetSequence(request.ApplicationId, request.SequenceId);
            var sectionsTask = _qnaApiClient.GetSections(request.ApplicationId, request.SequenceId);

            await Task.WhenAll(sequenceTask, sectionsTask);

            return new GetSequenceAndSectionsResponse
            {
                Sequence = sequenceTask.Result,
                Sections = sectionsTask.Result
            };
        }
    }
}
