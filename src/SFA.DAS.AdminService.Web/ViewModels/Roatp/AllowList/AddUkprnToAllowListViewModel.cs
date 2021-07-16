using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowList;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowList
{
    public class AddUkprnToAllowListViewModel
    {
        public string Ukprn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<AllowedUkprn> AllowedUkprns { get; set; }

        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
