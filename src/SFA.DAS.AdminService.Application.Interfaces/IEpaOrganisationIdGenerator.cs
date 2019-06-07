using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IEpaOrganisationIdGenerator
    {
        string GetNextOrganisationId();
        string GetNextContactUsername();
    }
}
