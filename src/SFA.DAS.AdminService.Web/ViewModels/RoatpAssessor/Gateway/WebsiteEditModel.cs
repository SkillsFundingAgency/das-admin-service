using Microsoft.AspNetCore.Mvc;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class WebsiteEditModel
    {
        [FromRoute]
        public Guid ApplicationId { get; set; }

        public OutcomeViewModel Outcome { get; set; }
    }
}
