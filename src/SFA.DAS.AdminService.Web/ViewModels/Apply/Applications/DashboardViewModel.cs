﻿using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class DashboardViewModel
    {
        public PaginatedList<ApplicationSummaryItem> Applications { get; set; }
    }
}