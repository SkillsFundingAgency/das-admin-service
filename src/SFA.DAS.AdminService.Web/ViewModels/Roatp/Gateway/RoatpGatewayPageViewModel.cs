using System;
using System.Collections.Generic;
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
     
        public Option OptionPass { get; set; }
        public string OptionPassText { get; set; }
        public Option OptionFail{ get; set; }
        public string OptionFailText { get; set; }

        public Option OptionInProgress { get; set; }
        public string OptionInProgressText { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }
    }


    public class TabularData
    {
        // public string Caption { get; set; }
        public List<string> HeadingTitles { get; set; }
        public List<TabularDataRow> DataRows { get; set; }
    }

    public class TabularDataRow
    {
        public string Id { get; set; }
        public List<string> Columns { get; set; }
        public string DetailsLabel { get; set; }
        public string DetailsValue { get; set; }
    }


    public class Option
    {
        public string Heading { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
