using AutoMapper;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using System;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpApprovalModelProfile : Profile
    {
        public RoatpApprovalModelProfile()
        {
            CreateMap<RoatpApplicationApprovalViewModel, CreateRoatpOrganisationRequest>()
                .ForMember(dest => dest.ProviderTypeId, opt => opt.MapFrom(source => source.ProviderTypeId))
                .ForMember(dest => dest.OrganisationTypeId, opt => opt.MapFrom(source => source.OrganisationTypeId))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.UKPRN))
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(source => source.LegalName))
                .ForMember(dest => dest.TradingName, opt => opt.MapFrom(source => source.TradingName))
                .ForMember(dest => dest.CharityNumber, opt => opt.MapFrom(source => source.CharityNumber))
                .ForMember(dest => dest.CompanyNumber, opt => opt.MapFrom(source => source.CompanyNumber))
                .ForMember(dest => dest.ApplicationDeterminedDate, opt => opt.MapFrom(source => source.ApplicationDeterminedDate))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(source => source.Username))
                .ForMember(dest => dest.StatusDate, opt => opt.UseValue<DateTime>(DateTime.Now))
                .ForMember(dest => dest.ParentCompanyGuarantee, opt => opt.UseValue<bool>(false))
                .ForMember(dest => dest.FinancialTrackRecord, opt => opt.UseValue<bool>(true))
                .ForMember(dest => dest.NonLevyContract, opt => opt.UseValue<bool>(false))
                .ForMember(dest => dest.SourceIsUKRLP, opt => opt.UseValue<bool>(true));
        }
    }
}
