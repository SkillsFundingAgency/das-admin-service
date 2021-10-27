using MediatR;
using SFA.DAS.AdminService.Web.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Commands.ReturnApplicationSequence
{
    public class ReturnApplicationSequenceHandler : IRequestHandler<ReturnApplicationSequenceCommand>
    {
        private readonly IApplicationService _applicationService;
       
        public ReturnApplicationSequenceHandler(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        
        public async Task<Unit> Handle(ReturnApplicationSequenceCommand request, CancellationToken cancellationToken)
        {
            await _applicationService.ReturnApplicationSequence(request.Id, request.SequenceNo, request.ReturnType, request.Username);

            return Unit.Value;
        }
    }
}
