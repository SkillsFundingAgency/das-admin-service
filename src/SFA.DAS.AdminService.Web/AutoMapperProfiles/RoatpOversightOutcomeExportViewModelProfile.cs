using AutoMapper;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpOversightOutcomeExportViewModelProfile : Profile
    {
        public RoatpOversightOutcomeExportViewModelProfile()
        {
            CreateMap<RoatpApplicationOversightDownloadItem, RoatpOversightOutcomeExportViewModel>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                .ForMember(dest => dest.ApplicationReferenceNumber, opt => opt.MapFrom(source => source.ApplicationReferenceNumber))
                .ForMember(dest => dest.ApplicationSubmittedDate, opt => opt.MapFrom(source => source.ApplicationSubmittedDate))
                .ForMember(dest => dest.OrganisationName, opt => opt.MapFrom(source => source.OrganisationName))
                .ForMember(dest => dest.IsOnRegister, opt => opt.MapFrom(source => !string.IsNullOrWhiteSpace(source.ProviderRouteNameOnRegister) && source.OrganisationStatus != OrganisationStatus.Removed))
                .ForMember(dest => dest.ProviderRouteNameOnRegister, opt => opt.MapFrom(source => source.ProviderRouteNameOnRegister))
                .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(source => source.OrganisationType))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.CompanyNumber));
        }
    }
}
