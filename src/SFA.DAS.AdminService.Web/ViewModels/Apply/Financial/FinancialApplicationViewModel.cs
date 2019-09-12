using System;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    public class FinancialApplicationViewModel
    {
        public string ApplicationReference { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }

        public Section Section { get; }
        public FinancialGrade Grade { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }
        public Guid OrgId { get; set; }

        public FinancialApplicationViewModel() { }

        public FinancialApplicationViewModel(Guid id, Guid applicationId, Section section, FinancialGrade grade, AssessorService.ApplyTypes.Application application)
        {
            Id = id;
            if (section != null)
            {
                Section = section;
                ApplicationId = section.ApplicationId;
            }
            else
            {
                ApplicationId = applicationId;
            }

            SetupGrade(section, grade);
            OrgId = application.ApplyingOrganisationId;

            if (application.ApplicationData != null)
            {
                ApplicationReference = application.ApplicationData.ReferenceNumber;
            }


            if (application.ApplyingOrganisation?.OrganisationData != null)
            {
                Ukprn = application.ApplyingOrganisation.EndPointAssessorUkprn;
                LegalName = application.ApplyingOrganisation.OrganisationData.LegalName;
                TradingName = application.ApplyingOrganisation.OrganisationData.TradingName;
                ProviderName = application.ApplyingOrganisation.OrganisationData.ProviderName;
                CompanyNumber = application.ApplyingOrganisation.OrganisationData.CompanyNumber;
            }
        }

        private void SetupGrade(Section section, FinancialGrade grade)
        {
            if (grade != null)
            {
                Grade = grade;
            }
            else
            {
                Grade = new FinancialGrade();
            }

            if (Grade.OutstandingFinancialDueDate is null) Grade.OutstandingFinancialDueDate = new FinancialDueDate();
            if (Grade.GoodFinancialDueDate is null) Grade.GoodFinancialDueDate = new FinancialDueDate();
            if (Grade.SatisfactoryFinancialDueDate is null) Grade.SatisfactoryFinancialDueDate = new FinancialDueDate();
        }
    }
}