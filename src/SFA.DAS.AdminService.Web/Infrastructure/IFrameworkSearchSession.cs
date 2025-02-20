namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IFrameworkSearchSession
    {
    } 

    public class FrameworkSearchSession : IFrameworkSearchSession
    {
        private readonly ISessionService _sessionService;

        public FrameworkSearchSession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
