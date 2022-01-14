﻿using SFA.DAS.AdminService.Web.Models.Merge;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class EpaoSearchResultsViewModel
    {
        public string OrganisationType { get; set; }
        public string SearchString { get; set; }
        public List<Epao> Results { get; set; }
        public SessionCommand PreviousCommand { get; set; }
    }
}
