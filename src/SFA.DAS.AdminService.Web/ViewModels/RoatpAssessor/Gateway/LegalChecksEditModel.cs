using Microsoft.AspNetCore.Mvc;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway
{
    public class LegalChecksEditModel
    {
        [FromRoute]
        public Guid ApplicationId { get; set; }

        public string LegalNameCheck { get; set; }
        public string StatusCheck { get; set; }
        public string AddressCheck { get; set; }

        public OutcomeViewModel Outcome { get; set; }
    }
}
