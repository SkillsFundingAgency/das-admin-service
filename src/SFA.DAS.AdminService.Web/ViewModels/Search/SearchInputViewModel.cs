using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class SearchInputViewModel
    {
        public string SearchString { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? Date { get; set; }
        public string SearchType { get; set; }

        public bool IsFrameworkSearchPopulated()
        {
            return !string.IsNullOrEmpty(FirstName) &&
                   !string.IsNullOrEmpty(LastName) &&
                   !string.IsNullOrEmpty(Day) &&
                   !string.IsNullOrEmpty(Month) &&
                   !string.IsNullOrEmpty(Year);
        }
    }
}
