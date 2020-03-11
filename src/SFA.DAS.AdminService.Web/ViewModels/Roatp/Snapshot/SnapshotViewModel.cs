using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Snapshot
{
    public class SnapshotViewModel
    {
        public Guid? ApplicationId { get; set; }
        public bool? SnapshotSuccessful { get; set; }
        public Guid? SnapshotApplicationId { get; set; }
    }
}
