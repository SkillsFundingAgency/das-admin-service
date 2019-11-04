using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Shared
{
    public class PaginationLinksViewModel
    {
        public PaginatedList PaginatedList { get; set; }

        public string ChangePageAction { get; set; }
        public string ChangePageController { get; set; }
        public string Fragment { get; set; }
    }
}
