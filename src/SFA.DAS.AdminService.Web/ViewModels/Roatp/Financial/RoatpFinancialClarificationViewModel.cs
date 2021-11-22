using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpFinancialClarificationViewModel : RoatpFinancialApplicationViewModel
    {
        public string Comments { get; set; }
        public IFormFileCollection FilesToUpload { get; set; }
        public string InternalComments { get; set; }
        public string ClarificationFile { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }
    }
}