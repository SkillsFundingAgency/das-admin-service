using AutoMapper;
using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpAssessorProfile : Profile
    {
        public RoatpAssessorProfile()
        {
            CreateMap<RoatpAssessor.Domain.DTOs.Application, DashboardNewApplication>();
        }
    }
}
