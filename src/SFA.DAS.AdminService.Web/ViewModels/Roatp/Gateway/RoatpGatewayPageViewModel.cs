using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{

    // this is a viewmodel template, can be removed once work done
    public class RoatpGatewayPageViewModel
    {
        public Guid ApplicationId { get; set; }
        public string PageId { get; set; }


        //public string Caption { get; set; }
        //public string Heading { get; set; }

       public string Ukrpn { get; set; }
        //public DateTime? ApplicationSubmittedOn { get; set; }
        //public DateTime SourcesCheckedOn { get; set; }

        public List<TabularData> Tables { get; set; }   // these may disappear as pages get created
        public TabularData SummaryList { get; set; }  // this may disappear as pages get created
        public string Value { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }

        public string OptionInProgressText { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }
    }
}
