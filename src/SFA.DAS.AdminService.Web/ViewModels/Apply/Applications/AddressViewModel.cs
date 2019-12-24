using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class AddressViewModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public string CommaSeperatedAddress()
        {
            return string.Join(',',  new List<string> {
                AddressLine1,
                AddressLine2,
                AddressLine3,
                AddressLine4,
                Postcode
            }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
