using SFA.DAS.AdminService.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.FrameworkSearch
{
    public class FrameworkSearchRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<FrameworkResultViewModel> FrameworkResults { get; set; }
        public int SelectedResult { get; set; }

        public void StartNewRequest()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            DateOfBirth = null;
            FrameworkResults = new List<FrameworkResultViewModel>();
            SelectedResult = 0;
        }
    }
}
