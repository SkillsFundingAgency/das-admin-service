using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Domain.Apply
{
    public class ApplicationDetails
    {
        public ApplicationResponse ApplicationResponse { get; set; }
        public Dictionary<string, object> ApplicationData { get; set; }
        public Organisation Organisation { get; set; }
        public List<Contact> OrganisationContacts { get; set; }
        public Contact ApplyingContact { get; set; }
    }
}