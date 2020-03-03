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
        public string NextPageId { get; set; }

        public string Caption { get; set; }
        public string Heading { get; set; }

        public TabularData TextListing { get; set; }
        public List<TabularData> Tables { get; set; }
        public TabularData SummaryList { get; set; }
        public string Value { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }

        public string OptionInProgressText { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }
    }
}
