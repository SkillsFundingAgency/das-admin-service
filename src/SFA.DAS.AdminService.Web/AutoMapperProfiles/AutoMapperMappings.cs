using AutoMapper;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Linq;
using OrganisationStatus = SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.OrganisationStatus;

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
                .ForMember(dest => dest.ContactId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(source => source.Title))
                .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(source => source.GivenNames))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(source => source.PhoneNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.EditPrivilegesViewModel, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedPrivileges, opt => opt.Ignore())
                .ForMember(dest => dest.ActionRequired, opt => opt.Ignore())
                .ForMember(dest => dest.AllPrivilegeTypes, opt => opt.Ignore())
                .ForMember(dest => dest.EndPointAssessorOrganisationId, opt => opt.Ignore());

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
            CreateMap<FrameworkSearchSession, FrameworkMultipleResultsViewModel>();
            CreateMap<SearchInputViewModel, FrameworkLearnerSearchRequest>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateExtensions.ConstructDate(src.Day, src.Month, src.Year)));
            CreateMap<FrameworkLearnerSearchResponse, FrameworkResultViewModel>();
            CreateMap<FrameworkSearchSession, SearchInputViewModel>()
                .ForMember(dest => dest.SearchType, opt => opt.MapFrom(src => SearchTypes.Frameworks))
                .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? 
                    (int?)src.DateOfBirth.Value.Day : null))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? 
                    (int?)src.DateOfBirth.Value.Month : null))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? 
                    (int?)src.DateOfBirth.Value.Year : null));
            CreateMap<GetFrameworkLearnerResponse, FrameworkLearnerDetailsViewModel>()
                .ForMember(dest => dest.Learner, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src =>
                    src.QualificationsAndAwardingBodies == null ? new List<string>() :
                    src.QualificationsAndAwardingBodies.Select(qualification =>
                        $"{qualification.Name}, {qualification.AwardingBody}").ToList()));
            CreateMap<FrameworkSearchSession, FrameworkReprintReasonViewModel>()
                .ForMember(dest => dest.ApprenticeName, opt => opt.MapFrom(src=> $"{src.FirstName} {src.LastName}"));
            CreateMap<FrameworkSearchSession, FrameworkReprintAddressViewModel>();
            CreateMap<GetFrameworkLearnerResponse, FrameworkReprintReasonViewModel>()
                .ForMember(dest => dest.ApprenticeName, opt => opt.MapFrom(src=> $"{src.ApprenticeForename} {src.ApprenticeSurname}"));

            CreateMap<StaffBatchSearchResult, StaffBatchSearchResultViewModel>()
                .ConstructUsing(src => new StaffBatchSearchResultViewModel(src));
            CreateMap<StaffBatchSearchResponse, BatchSearchViewModel<StaffBatchSearchResultViewModel>>()
                .ForMember(dest => dest.PaginatedList, opt => opt.MapFrom((src, dest, _, context) => new PaginatedList<StaffBatchSearchResultViewModel>(
                    src.Results.Items.Select(item => context.Mapper.Map<StaffBatchSearchResultViewModel>(item)).ToList(),
                    src.Results.TotalRecordCount,
                    src.Results.PageIndex,
                    src.Results.PageSize
                )));
        }
    }
}
