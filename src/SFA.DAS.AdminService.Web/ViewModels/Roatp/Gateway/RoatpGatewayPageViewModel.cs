using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayPageViewModel
    {
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }

        public string Status { get; set; }

        public string Ukprn { get; set; }
        public string UkrlpLegalName { get; set; }

        // It will not be needed any more. It was for the breadcrumb, but the logic for it changed
        public string GatewayReviewStatus { get; set; }

        // these two will disappear once views use them locally - retained to keep Greg's UX stuff
         public List<TabularData> Tables { get; set; }   
         //the SummaryList items are only here to preserve the UX stuff Greg did, and these will disappear once we've done the stories with 1+ tables (section 2)
        public TabularData SummaryList { get; set; }  // this will disappear once section 2 work starts
     
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }

        public string OptionInProgressText { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public string Heading { get; set; }
        public string Caption { get; set; }
    }
}
