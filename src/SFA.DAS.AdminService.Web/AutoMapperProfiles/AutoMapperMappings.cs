using AutoMapper;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class AutoMapperMappings : Profile
    {
        public AutoMapperMappings()
        {
            CreateMap<RoatpApplicationOversightDownloadItem, RoatpOversightOutcomeExportViewModel>()
                    .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
                    .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))
                    .ForMember(dest => dest.ApplicationReferenceNumber, opt => opt.MapFrom(source => source.ApplicationReferenceNumber))
                    .ForMember(dest => dest.ApplicationSubmittedDate, opt => opt.MapFrom(source => source.ApplicationSubmittedDate))
                    .ForMember(dest => dest.OrganisationName, opt => opt.MapFrom(source => source.OrganisationName))
                    .ForMember(dest => dest.IsOnRegister, opt => opt.MapFrom(source => !string.IsNullOrWhiteSpace(source.ProviderRouteNameOnRegister) && source.OrganisationStatusId != OrganisationStatus.Removed))
                    .ForMember(dest => dest.ProviderRouteNameOnRegister, opt => opt.MapFrom(source => (!string.IsNullOrWhiteSpace(source.ProviderRouteNameOnRegister) && source.OrganisationStatusId != OrganisationStatus.Removed) ? source.ProviderRouteNameOnRegister : string.Empty))
                    .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(source => source.OrganisationType))
                    .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.CompanyNumber));

            CreateMap<ContactResponse, RegisterViewAndEditUserViewModel>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.ContactId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(source => source.Title))
                .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(source => source.GivenNames))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(source => source.PhoneNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status));

            CreateMap<AddOrganisationViewModel, AddOrganisationProviderTypeViewModel>();
            CreateMap<AddOrganisationViewModel, AddOrganisationTypeViewModel>();
            CreateMap<RoatpApplicationOversightDownloadItem, RoatpOversightOutcomeExportViewModel>();
            CreateMap<UpdateOrganisationLegalNameViewModel, UpdateOrganisationLegalNameRequest>();
            CreateMap<UpdateOrganisationUkprnViewModel, UpdateOrganisationUkprnRequest>();
            CreateMap<UpdateOrganisationCompanyNumberViewModel, UpdateOrganisationCompanyNumberRequest>();
            CreateMap<UpdateOrganisationStatusViewModel, UpdateOrganisationStatusRequest>();
            CreateMap<UpdateOrganisationTypeViewModel, UpdateOrganisationTypeRequest>();
            CreateMap<UpdateOrganisationTradingNameViewModel, UpdateOrganisationTradingNameRequest>();
            CreateMap<UpdateOrganisationParentCompanyGuaranteeViewModel, UpdateOrganisationParentCompanyGuaranteeRequest>();
            CreateMap<UpdateOrganisationFinancialTrackRecordViewModel, UpdateOrganisationFinancialTrackRecordRequest>();
            CreateMap<UpdateOrganisationProviderTypeViewModel, UpdateOrganisationProviderTypeRequest>();
            CreateMap<UpdateOrganisationCharityNumberViewModel, UpdateOrganisationCharityNumberRequest>();
            CreateMap<UpdateApplicationDeterminedDateViewModel, UpdateOrganisationApplicationDeterminedDateRequest>();
        }
    }
}
