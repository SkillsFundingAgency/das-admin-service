﻿using System;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApiClient.Models
{
    public class CreateGatewayModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime AssignedAt { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
    }
}