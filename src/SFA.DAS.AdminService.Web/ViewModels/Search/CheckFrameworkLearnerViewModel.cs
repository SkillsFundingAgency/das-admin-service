using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class CheckFrameworkLearnerViewModel 
    {
        public FrameworkLearnerViewModel LearnerDetails { get; set; }
        public UpdateReprintReasonViewModel ReprintDetails { get; set; }
        public FrameworkLearnerAddressViewModel AddressDetails { get; set; }
    }
}