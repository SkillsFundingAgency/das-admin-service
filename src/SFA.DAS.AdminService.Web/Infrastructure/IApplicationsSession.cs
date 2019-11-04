using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplicationsSession
    {
        bool ApplicationsSessionValid { get; set; }
        ApplicationsState NewOrganisationApplications { get; }
        ApplicationsState InProgressOrganisationApplications { get; }
        ApplicationsState FeedbackOrganisationApplications { get; }
        ApplicationsState ApprovedOrganisationApplications { get; }
    } 

    public class ApplicationsSession : IApplicationsSession
    {
        private readonly ISessionService _sessionService;

        private const string _uniqueKeyNewOrganisationApplications = "NewOrganisationApplications";
        private const string _uniqueKeyInProgressOrganisationApplications = "InProgressOrganisationApplications";
        private const string _uniqueKeyFeedbackOrganisationApplications = "FeedbackOrganisationApplications";
        private const string _uniqueKeyApprovedOrganisationApplications = "ApprovedOrganisationApplications";

        public ApplicationsSession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public bool ApplicationsSessionValid
        {
            get
            {
                return _sessionService.Get<bool>("ApplicationsSessionValid");
            }
            set
            {
                _sessionService.Set("ApplicationsSessionValid", value);
            }
        }

        public ApplicationsState NewOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, _uniqueKeyNewOrganisationApplications);
            }
        }

        public ApplicationsState InProgressOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, _uniqueKeyInProgressOrganisationApplications);
            }
        }

        public ApplicationsState FeedbackOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, _uniqueKeyFeedbackOrganisationApplications);
            }
        }

        public ApplicationsState ApprovedOrganisationApplications
        {
            get
            {
                return new ApplicationsState(_sessionService, _uniqueKeyApprovedOrganisationApplications);
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
        
        public OrganisationApplicationsSortColumn SortColumn
        {
            get
            {
                return _sessionService.Get<OrganisationApplicationsSortColumn>(_uniqueKey + "_SortColumn");
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
