using Microsoft.AspNetCore.Mvc;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class LegalChecksEditModel
    {
        [FromRoute]
        public Guid applicationId { get; set; }

        public bool? LegalNameMatch { get; set; }

        public OutcomeViewModel Outcome { get; set; }
    }
}
