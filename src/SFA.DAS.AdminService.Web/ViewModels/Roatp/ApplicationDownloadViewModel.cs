using System;
using SFA.DAS.AdminService.Web.Attributes;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    public class ApplicationDownloadViewModel
    {
        [SuppressBindingErrors]
        public DateTime? FromDate { get; set; }
        [SuppressBindingErrors]
        public DateTime? ToDate { get; set; }
    }
}
