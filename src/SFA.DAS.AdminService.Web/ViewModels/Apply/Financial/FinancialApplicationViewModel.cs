using System;
using SFA.DAS.AssessorService.ApplyTypes;

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

        public ApplicationSection Section { get; }
        public FinancialApplicationGrade Grade { get; set; }
        public Guid ApplicationId { get; set; }

        public FinancialApplicationViewModel() { }

        public FinancialApplicationViewModel(Guid applicationId, ApplicationSection section, FinancialApplicationGrade grade, AssessorService.ApplyTypes.Application application)
        {
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

            if (application != null)
            {
                if (application.ApplicationData != null)
                {
                    ApplicationReference = application.ApplicationData.ReferenceNumber;
                }


                if (application.ApplyingOrganisation?.OrganisationDataFromJson != null)
                {
                    Ukprn = application.ApplyingOrganisation.EndPointAssessorUkprn;
                    LegalName = application.ApplyingOrganisation.OrganisationDataFromJson.LegalName;
                    TradingName = application.ApplyingOrganisation.OrganisationDataFromJson.TradingName;
                    ProviderName = application.ApplyingOrganisation.OrganisationDataFromJson.ProviderName;
                    CompanyNumber = application.ApplyingOrganisation.OrganisationDataFromJson.CompanyNumber;
                }
            }
        }

        private void SetupGrade(ApplicationSection section, FinancialApplicationGrade grade)
        {
            if (grade != null)
            {
                Grade = grade;
            }
            else if (section?.QnAData?.FinancialApplicationGrade != null)
            {
                Grade = section.QnAData.FinancialApplicationGrade;
            }
            else
            {
                Grade = new FinancialApplicationGrade();
            }

            if (Grade.OutstandingFinancialDueDate is null) Grade.OutstandingFinancialDueDate = new FinancialDueDate();
            if (Grade.GoodFinancialDueDate is null) Grade.GoodFinancialDueDate = new FinancialDueDate();
            if (Grade.SatisfactoryFinancialDueDate is null) Grade.SatisfactoryFinancialDueDate = new FinancialDueDate();
        }
    }
}