using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Search
{
    public class FrameworkSearchSession
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<FrameworkLearnerSummaryViewModel> FrameworkResults { get; set; }
        public Guid? SelectedResult { get; set; }

        //FrameworkReprintReason reason
        public string CertificateNumber { get; set; }
        public string Status { get; set; }
        public System.DateTime DateSentToPrinter { get; set; }
        public string TicketNumber { get; set; }
        public List<string> SelectedReprintReasons { get; set; }
        public string OtherReason { get; set; }

        //Address
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}
