using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public class FrameworkSearchOrchestrator : IFrameworkSearchOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IFrameworkSearchSessionService _sessionStorage;
        private readonly IHttpContextAccessor _contextAccessor;

        public FrameworkSearchOrchestrator(IMediator mediator, IFrameworkSearchSessionService sessionStorage,
            IHttpContextAccessor contextAccessor)
        {
            _mediator = mediator;
            _sessionStorage = sessionStorage;
            _contextAccessor = contextAccessor;
        }
    }

}
