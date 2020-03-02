using System.Collections.Generic;

namespace SFA.DAS.AdminService.Application.Models
{
    public class TabularDataRow
    {
        public string Id { get; set; }
        public List<string> Columns { get; set; }
        public string DetailsLabel { get; set; }
        public string DetailsValue { get; set; }
    }
}
