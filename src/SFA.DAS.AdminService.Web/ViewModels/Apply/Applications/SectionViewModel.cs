using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SectionViewModel
    {
        public string ApplicationReference { get; set; }
        public AssessorService.ApplyTypes.FinancialGrade Grade { get; set; }
        public string StandardName { get; set; }
        public int StandardCode { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public string CompanyNumber { get; set; }

        public string Title { get; }
        public string Status { get; set; }

        public Section Section { get; }
        public ApplySection ApplySection { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }

        public bool? IsSectionComplete { get; set; }

        public SectionViewModel(ApplicationResponse application, Organisation organisation, Section section, ApplySection applySection)
        {
            ApplicationId = application.Id;
            ApplicationReference = application.ApplyData.Apply.ReferenceNumber;
            Grade = application.financialGrade;
            StandardName = application.ApplyData.Apply.StandardName;
            StandardCode = application.ApplyData.Apply.StandardCode;

            LegalName = organisation.OrganisationData.LegalName;
            TradingName = organisation.OrganisationData.TradingName;
            ProviderName = organisation.OrganisationData.ProviderName;
            Ukprn = organisation.EndPointAssessorUkprn;
            CompanyNumber = organisation.OrganisationData.CompanyNumber;

            Section = section;
            ApplySection = applySection;
            SequenceNo = section.SequenceNo;
            SectionNo = section.SectionNo;
            Title = section.Title;
            Status = section.Status;

            if (section.Status == ApplicationSectionStatus.Evaluated)
            {
                IsSectionComplete = true;
            }
        }
    }
}
