using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Automapper.CustomResolvers;
using SFA.DAS.AdminService.Web.ViewModels.Private;
using SFA.DAS.AdminService.Web.Automapper;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CertificateSummaryResponse, CertificateDetailApprovalViewModel>()
                    .ForMember(
                        dest => dest.IsApproved, opt => opt.MapFrom(src => src.Status)
                    );
            });
        }
    }
}