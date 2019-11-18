using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Shared
{
    public class PaginationViewModel<T>
    {
        public PaginatedList<T> PaginatedList { get; set; }

        public int ItemsPerPage { get; set; }

        public string SortColumn { get; set; }

        public string SortDirection { get; set; }

        public string Fragment { get; set; }

        public string Title { get; set; }

        public string ChangePageAction { get; set; }

        public string SortColumnAction { get; set; }

        public string ChangeItemsPerPageAction { get; set; }

        public List<SelectListItem> ItemsPerPageList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "50", Text = "50" },
            new SelectListItem { Value = "100", Text = "100"  },
            new SelectListItem { Value = "500", Text = "500"  }
        };
    }
}
