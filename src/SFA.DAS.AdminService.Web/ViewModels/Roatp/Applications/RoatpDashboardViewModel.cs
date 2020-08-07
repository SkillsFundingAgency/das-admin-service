﻿using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpDashboardViewModel
    {
        public PaginatedList<RoatpApplicationSummaryItem> Applications { get; set; }
    }
}
