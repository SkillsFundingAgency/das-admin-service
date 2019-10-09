using AutoMapper;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RegisterViewAndEditUserViewModelProfile : Profile
    {
        public RegisterViewAndEditUserViewModelProfile()
        {
            CreateMap<ContactResponse, RegisterViewAndEditUserViewModel>()
                .ForMember(dest => dest.ContactId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(source => source.Title))
                .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(source => source.GivenNames))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(source => source.PhoneNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}

