using System;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Models
{
    public class EditPrivilegesViewModel
    {
        public Guid ContactId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public PrivilegeViewModel[] PrivilegeViewModels { get; set; }
        public string Button { get; set; }
    }

    public class PrivilegeViewModel
    {
        public Privilege Privilege { get; set; }
        public bool Selected { get; set; }
    }
}