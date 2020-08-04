using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using System;

namespace SFA.DAS.AdminService.Web.Models
{
    public class BatchSearchViewModel<T>
    {
        public int? BatchNumber { get; set; }
        public int Page { get; set; }
        public DateTime SentToPrinterDate { get; set; }
        public DateTime? PrintedDate { get; set; }
        public PaginatedList<T> PaginatedList { get; set; }
    }
}
