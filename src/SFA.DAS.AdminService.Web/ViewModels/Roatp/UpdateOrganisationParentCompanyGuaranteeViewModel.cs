using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp
{
    public class UpdateOrganisationParentCompanyGuaranteeViewModel
    {
        public Guid OrganisationId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public string UpdatedBy { get; set; }
        public string LegalName { get; set; }
    }
}
