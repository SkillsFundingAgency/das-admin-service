using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Configuration
{
    public class GatewayPageConfiguration
    {

        public string PageId { get; set; }
        public string NextPageId { get; set; }

        public string Caption { get; set; }
        public string Heading { get; set; }
        public List<DataRow> TextListing { get; set; }
        public List<Table> Tables { get; set; }
        public List<DataRow> SummaryList { get; set; }
    }

    public class DataRow
    {
        public List<DataRowElement> Elements { get; set; }
        public string DetailsLabel { get; set; }
        public string DetailsValue { get; set; }
        public string DetailsKey { get; set; }
    }


    public class Table
    {
        // public string Caption { get; set; }
        public List<string> HeadingTitles { get; set; }
        public List<DataRow> DataRows { get; set; }
    }
    public class DataRowElement
    {
        public string Source { get; set; }   //should be one of the fields from DataSource Type:
                                             //"Text",
                                             //"ApplyQuestionTag",
                                             // "ApplySspqId,
                                             //"ApplySpecial" (eg submitted on date, sources checked on),
                                             //"UKRLP","CompaniesHouse","CharityCommission"
        public string Value { get; set; }
        public string Key { get; set; }
        // When ApplyQuestionTag, this contains the tag
        // when ApplySspqId, this contains a stringbag?  eg 1::2::21::YO-21 to get ultimate parent company details
        // when ApplySpecial, contains word that directs service to go get the value
        // When UKRLP the words used in the RoatpApi subservice to identify the details requested, and throw back the details in a formatted way
        // When Companies House, the words used in the ApplyApi subservice to identify the details requested, and throw back the details in a formatted way
        // When Charity Commission, the words used in the ApplyApi subservice to identify the details requested, and throw back the details in a formated way

    }


    public class DataSourceType
    {
        public string Text => "Text";
        public string ApplyQuestionTag => "ApplyQuestionTag";
        public string ApplySspqId => "ApplySspqId";
        public string UKRLP => "UKRLP";
        public string CompaniesHouse => "CompaniesHouse";
        public string CharityCommission => "CharityCommission";
    }
}
