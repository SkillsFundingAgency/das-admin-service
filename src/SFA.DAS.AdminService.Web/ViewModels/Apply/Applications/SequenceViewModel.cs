using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SequenceViewModel
    {
        public SequenceViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence, List<Section> sections, List<ApplySection> applySections)
        {
            ApplicationId = application.Id;
            ApplicationReference = application.ApplyData.Apply.ReferenceNumber;
            StandardName = application.ApplyData.Apply.StandardName;
            StandardCode = application.ApplyData.Apply.StandardCode;

            FinancialReviewStatus = application.FinancialReviewStatus;
            FinancialDueDate = application.financialGrade?.FinancialDueDate;

            LegalName = organisation.OrganisationData.LegalName;
            TradingName = organisation.OrganisationData.TradingName;
            ProviderName = organisation.OrganisationData.ProviderName;
            Ukprn = organisation.EndPointAssessorUkprn;
            CompanyNumber = organisation.OrganisationData.CompanyNumber;

            Sections = sections;
            ApplySections = applySections;
            SequenceNo = sequence.SequenceNo;
            Status = sequence.Status;
        }

        public string ApplicationReference { get; set; }
        public string StandardName { get; set; }
        public int StandardCode { get; set; }

        public string FinancialReviewStatus { get; set; }
        public DateTime? FinancialDueDate { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public string CompanyNumber { get; set; }

        public string Status { get; set; }
        public List<Section> Sections { get; }
        public List<ApplySection> ApplySections { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
    }
}
