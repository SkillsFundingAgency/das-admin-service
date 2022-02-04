using SFA.DAS.AdminService.Web.Models.Merge;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class SetSecondaryEpaoEffectiveToDateViewModel
    {
        public string SecondaryEpaoName { get; set; }
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public SetSecondaryEpaoEffectiveToDateViewModel() { }

        public SetSecondaryEpaoEffectiveToDateViewModel(Epao secondaryEpao, DateTime? effectiveTo)
        {
            SecondaryEpaoName = secondaryEpao?.Name;
            Day = effectiveTo.HasValue ? effectiveTo.Value.Day.ToString() : "";
            Month = effectiveTo.HasValue ? effectiveTo.Value.Month.ToString() : "";
            Year = effectiveTo.HasValue ? effectiveTo.Value.Year.ToString() : "";
        }
    }
}
