using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface ICharityCommissionApiClient
    {
        Task<Charity> GetCharityDetails(int charityNumber);
    }
}
