using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders
{
    public class AddUkprnToAllowedProvidersListViewModel
    {
        public string Ukprn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<AllowedProvider> AllowedProviders { get; set; }

        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
