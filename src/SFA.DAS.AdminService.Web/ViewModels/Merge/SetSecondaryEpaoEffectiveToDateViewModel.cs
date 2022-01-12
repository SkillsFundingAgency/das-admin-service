using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class SetSecondaryEpaoEffectiveToDateViewModel
    {
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public SetSecondaryEpaoEffectiveToDateViewModel(){ }

        public SetSecondaryEpaoEffectiveToDateViewModel(DateTime? effectiveTo)
        {
            Day = effectiveTo.HasValue ? effectiveTo.Value.Day.ToString() : "";
            Month = effectiveTo.HasValue ? effectiveTo.Value.Month.ToString() : "";
            Year = effectiveTo.HasValue ? effectiveTo.Value.Year.ToString() : "";
        }
    }
}
