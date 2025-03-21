using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkLearnerAddressViewModel 
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string BackAction { get; set; }
        public string AddressDisplay
        {
            get
            {
                var addressLines = new List<string>();

                if (!string.IsNullOrEmpty(AddressLine1))
                {
                    addressLines.Add(AddressLine1);
                }

                if (!string.IsNullOrEmpty(AddressLine2))
                {
                    addressLines.Add(AddressLine2);
                }

                if (!string.IsNullOrEmpty(TownOrCity))
                {
                    addressLines.Add(TownOrCity);
                }

                if (!string.IsNullOrEmpty(County))
                {
                    addressLines.Add(County);
                }

                if (!string.IsNullOrEmpty(Postcode))
                {
                    addressLines.Add(Postcode);
                }

                return string.Join("<br />", addressLines);
            }
        }
    }
}