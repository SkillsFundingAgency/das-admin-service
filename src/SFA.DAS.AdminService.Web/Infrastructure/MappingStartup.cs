using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<RegisterViewAndEditUserViewModelProfile>();
                cfg.AddProfile<RoatpOversightOutcomeExportViewModelProfile>();
            });
        }
    }
}