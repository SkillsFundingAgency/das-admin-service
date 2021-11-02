using MediatR;
using System;

namespace SFA.DAS.AdminService.Web.Queries.GetSequenceAndSections
{
    public class GetSequenceAndSectionsQuery : IRequest<GetSequenceAndSectionsResponse>
    {
        public Guid ApplicationId { get; set; }
        public Guid SequenceId { get; set; }

        public GetSequenceAndSectionsQuery(Guid applicationId, Guid sequenceId)
        {
            ApplicationId = applicationId;
            SequenceId = sequenceId;
        }
    }
}
