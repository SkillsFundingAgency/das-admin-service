using SFA.DAS.AdminService.Web.ViewModels.Register;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IPagingState
    {
        int ItemsPerPage { get; set; }
        string SortColumn { get; set; }
        string SortDirection { get; set; }
        int PageIndex { get; set; }
    }

    public interface IControllerSession
    {
        bool OrganisationApplication_SessionValid { get; set; }
        IPagingState OrganisationApplication_NewApplications { get; }
        IPagingState OrganisationApplication_InProgressApplications { get; }
        IPagingState OrganisationApplication_FeedbackApplications { get; }
        IPagingState OrganisationApplication_ApprovedApplications { get; }

        bool StandardApplication_SessionValid { get; set; }
        IPagingState StandardApplication_NewApplications { get; }
        IPagingState StandardApplication_InProgressApplictions { get; }
        IPagingState StandardApplication_FeedbackApplications { get; }
        IPagingState StandardApplication_ApprovedApplications { get; }

        bool WithdrawalApplication_SessionValid { get; set; }
        IPagingState WithdrawalApplication_NewApplications { get; }
        IPagingState WithdrawalApplication_InProgressApplications { get; }
        IPagingState WithdrawalApplication_FeedbackApplications { get; }
        IPagingState WithdrawalApplication_ApprovedApplications { get; }

        bool Register_SessionValid { get; set; }
        IPagingState Register_ApprovedStandards { get; }

        RegisterAddOrganisationStandardViewModel AddOrganisationStandardViewModel { get; set; }
        void Remove(string key);
    } 

    public class ControllerSession : IControllerSession
    {
        private readonly ISessionService _sessionService;

        public ControllerSession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public bool OrganisationApplication_SessionValid
        {
            get
            {
                return _sessionService.Get<bool>("OrganisationApplication_SessionValid");
            }
            set
            {
                _sessionService.Set("OrganisationApplication_SessionValid", value);
            }
        }

        public IPagingState OrganisationApplication_NewApplications
        {
            get
            {
                return new PagingState(_sessionService, "OrganisationApplication_NewApplications");
            }
        }

        public IPagingState OrganisationApplication_InProgressApplications
        {
            get
            {
                return new PagingState(_sessionService, "OrganisationApplication_InProgressApplications");
            }
        }

        public IPagingState OrganisationApplication_FeedbackApplications
        {
            get
            {
                return new PagingState(_sessionService, "OrganisationApplication_FeedbackApplications");
            }
        }

        public IPagingState OrganisationApplication_ApprovedApplications
        {
            get
            {
                return new PagingState(_sessionService, "OrganisationApplication_ApprovedApplications");
            }
        }

        public bool StandardApplication_SessionValid
        {
            get
            {
                return _sessionService.Get<bool>("StandardApplication_SessionValid");
            }
            set
            {
                _sessionService.Set("StandardApplication_SessionValid", value);
            }
        }

        public IPagingState StandardApplication_NewApplications
        {
            get
            {
                return new PagingState(_sessionService, "StandardApplication_NewApplications");
            }
        }

        public IPagingState StandardApplication_InProgressApplictions
        {
            get
            {
                return new PagingState(_sessionService, "StandardApplication_InProgressApplictions");
            }
        }

        public IPagingState StandardApplication_FeedbackApplications
        {
            get
            {
                return new PagingState(_sessionService, "StandardApplication_FeedbackApplications");
            }
        }

        public IPagingState StandardApplication_ApprovedApplications
        {
            get
            {
                return new PagingState(_sessionService, "StandardApplication_ApprovedApplications");
            }
        }

        public bool WithdrawalApplication_SessionValid
        {
            get
            {
                return _sessionService.Get<bool>("WithdrawalApplication_SessionValid");
            }
            set
            {
                _sessionService.Set("WithdrawalApplication_SessionValid", value);
            }
        }

        public IPagingState WithdrawalApplication_NewApplications
        {
            get
            {
                return new PagingState(_sessionService, "WithdrawalApplication_NewApplications");
            }
        }

        public IPagingState WithdrawalApplication_InProgressApplications
        {
            get
            {
                return new PagingState(_sessionService, "WithdrawalApplication_InProgressApplications");
            }
        }

        public IPagingState WithdrawalApplication_FeedbackApplications
        {
            get
            {
                return new PagingState(_sessionService, "WithdrawalApplication_FeedbackApplications");
            }
        }

        public IPagingState WithdrawalApplication_ApprovedApplications
        {
            get
            {
                return new PagingState(_sessionService, "WithdrawalApplication_ApprovedApplications");
            }
        }

        public bool Register_SessionValid
        {
            get
            {
                return _sessionService.Get<bool>("Register_SessionValid");
            }
            set
            {
                _sessionService.Set("Register_SessionValid", value);
            }
        }

        public IPagingState Register_ApprovedStandards
        {
            get
            {
                return new PagingState(_sessionService, "Register_ApprovedStandards");
            }
        }

        public RegisterAddOrganisationStandardViewModel AddOrganisationStandardViewModel
        {
            get
            {
                return _sessionService.Get<RegisterAddOrganisationStandardViewModel>("AddOrganisationStandardViewModel");
            }
            set
            {
                _sessionService.Set("AddOrganisationStandardViewModel", value);
            }
        }

        public void Remove(string key)
        {
            _sessionService.Remove(key);
        }
    }

    public class PagingState : IPagingState
    {
        private readonly ISessionService _sessionService;
        private readonly string _uniqueKey;

        public PagingState(ISessionService sessionService, string uniqueKey)
        {
            _sessionService = sessionService;
            _uniqueKey = uniqueKey;
        }

        public int ItemsPerPage
        {
            get
            {
                return _sessionService.Get<int>(_uniqueKey + "_ItemsPerPage");
            }
            set
            {
                _sessionService.Set(_uniqueKey + "_ItemsPerPage", value);
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
