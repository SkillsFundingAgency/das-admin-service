﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.AutoMapperProfiles
{
    public class RoatpFinancialSummaryExportViewModelProfile : Profile
    {
        public RoatpFinancialSummaryExportViewModelProfile()
        {
            CreateMap<RoatpFinancialSummaryDownloadItem, RoatpFinancialSummaryExportViewModel>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
                .ForMember(dest => dest.ApplicationReference, opt => opt.MapFrom(source => source.ApplicationReferenceNumber))
                .ForMember(dest => dest.Route, opt => opt.MapFrom(source => source.ApplicationRoute))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(source => source.OrganisationName))
                .ForMember(dest => dest.Ukprn, opt => opt.MapFrom(source => source.Ukprn))

                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(source => source.SubmittedDate))
                
                //todo: gateway pass date
                
                //.ForMember(dest => dest.CharityNo, opt => opt.MapFrom(source => source))


                .ForMember(dest => dest.TurnOver, opt => opt.MapFrom(source => source.FinancialData.TurnOver))
                .ForMember(dest => dest.Depreciation, opt => opt.MapFrom(source => source.FinancialData.Depreciation))
                .ForMember(dest => dest.ProfitLoss, opt => opt.MapFrom(source => source.FinancialData.ProfitLoss))
                .ForMember(dest => dest.Dividends, opt => opt.MapFrom(source => source.FinancialData.Dividends))
                .ForMember(dest => dest.IntangibleAssets, opt => opt.MapFrom(source => source.FinancialData.IntangibleAssets))
                .ForMember(dest => dest.Assets, opt => opt.MapFrom(source => source.FinancialData.Assets))
                .ForMember(dest => dest.Liabilities, opt => opt.MapFrom(source => source.FinancialData.Liabilities))
                .ForMember(dest => dest.ShareholderFunds, opt => opt.MapFrom(source => source.FinancialData.ShareholderFunds))
                .ForMember(dest => dest.Borrowings, opt => opt.MapFrom(source => source.FinancialData.Borrowings))

                //                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
                //.ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))

                ;

            //.ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
            //.ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))

            //.ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
