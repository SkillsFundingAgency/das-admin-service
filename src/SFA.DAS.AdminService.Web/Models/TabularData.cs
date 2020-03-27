using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Models
{
    public class TabularData
    {
        public string Caption { get; set; }
        public List<string> HeadingTitles { get; set; }
        public List<TabularDataRow> DataRows { get; set; }
    }
}
