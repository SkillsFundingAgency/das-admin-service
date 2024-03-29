﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
{
    public class GetAddressRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetAddressRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
