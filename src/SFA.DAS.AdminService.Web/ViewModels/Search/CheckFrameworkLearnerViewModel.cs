using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class CheckFrameworkLearnerViewModel 
    {
        public FrameworkLearnerDetailsViewModel LearnerDetails { get; set; }
        public AmendFrameworkReprintReasonViewModel ReprintDetails { get; set; }
        public FrameworkLearnerAddressViewModel AddressDetails { get; set; }
    }
}