namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplicationsSession
    {
        bool OrganisationApplicationsSessionValid { get; set; }
        ApplicationsState NewOrganisationApplications { get; }
        ApplicationsState InProgressOrganisationApplications { get; }
        ApplicationsState FeedbackOrganisationApplications { get; }
        ApplicationsState ApprovedOrganisationApplications { get; }

        bool StandardApplicationsSessionValid { get; set; }
        ApplicationsState NewStandardApplications { get; }
        ApplicationsState InProgressStandardApplications { get; }
        ApplicationsState FeedbackStandardApplications { get; }
        ApplicationsState ApprovedStandardApplications { get; }
    } 

    public class ApplicationsSession : IApplicationsSession
    {
        private readonly ISessionService _sessionService;

        public ApplicationsSession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public bool OrganisationApplicationsSessionValid
        {
            get
            {
                return _sessionService.Get<bool>("OrganisationApplicationsSessionValid");
            }
            set
            {
                _sessionService.Set("OrganisationApplicationsSessionValid", value);
            }
        }

        public ApplicationsState NewOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "NewOrganisationApplications");
            }
        }

        public ApplicationsState InProgressOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "InProgressOrganisationApplications");
            }
        }

        public ApplicationsState FeedbackOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "FeedbackOrganisationApplications");
            }
        }

        public ApplicationsState ApprovedOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "ApprovedOrganisationApplications");
            }
        }

        public bool StandardApplicationsSessionValid
        {
            get
            {
                return _sessionService.Get<bool>("StandardApplicationsSessionValid");
            }
            set
            {
                _sessionService.Set("StandardApplicationsSessionValid", value);
            }
        }

        public ApplicationsState NewStandardApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "NewStandardApplications");
            }
        }

        public ApplicationsState InProgressStandardApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "InProgressStandardApplications");
            }
        }

        public ApplicationsState FeedbackStandardApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "FeedbackStandardApplications");
            }
        }

        public ApplicationsState ApprovedStandardApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, "ApprovedStandardApplications");
            }
        }
    }

    public class ApplicationsState
    {
        private readonly ISessionService _sessionService;
        private readonly string _uniqueKey;

        public ApplicationsState(ISessionService sessionService, string uniqueKey)
        {
            _sessionService = sessionService;
            _uniqueKey = uniqueKey;
        }

        public int ApplicationsPerPage
        {
            get
            {
                return _sessionService.Get<int>(_uniqueKey + "_ApplicationsPerPage");
            }
            set
            {
                _sessionService.Set(_uniqueKey + "_ApplicationsPerPage", value);
            }
        }
        
        public string SortColumn
        {
            get
            {
                return _sessionService.Get(_uniqueKey + "_SortColumn");
            }
            set
            {
                _sessionService.Set(_uniqueKey + "_SortColumn", value);
            }
        }

        public string SortDirection
        {
            get
            {
                return _sessionService.Get(_uniqueKey + "_SortDirection");
            }
            set
            {
                _sessionService.Set(_uniqueKey + "_SortDirection", value);
            }
        }

        public int PageIndex
        {
            get
            {
                return _sessionService.Get<int>(_uniqueKey + "_PageIndex");
            }
            set
            {
                _sessionService.Set(_uniqueKey + "_PageIndex", value);
            }
        }
    }
}
