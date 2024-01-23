using System;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class FinancialApplicationViewModel
    {
        public string ApplicationReference { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }

        public Section Section { get; }
        public AssessorService.Domain.Entities.FinancialGrade Grade { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }
        public Guid OrgId { get; set; }

        public FinancialDueDate OutstandingFinancialDueDate { get; set; }
        public FinancialDueDate GoodFinancialDueDate { get; set; }
        public FinancialDueDate SatisfactoryFinancialDueDate { get; set; }
        public FinancialDueDate MonitoringFinancialDueDate { get; set; }

        public FinancialApplicationViewModel() { }

        public FinancialApplicationViewModel(Guid id, Guid applicationId, Section section, AssessorService.Domain.Entities.FinancialGrade grade, AssessorService.ApplyTypes.Application application)
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

            SetupGradeAndFinancialDueDate(grade);
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

        private void SetupGradeAndFinancialDueDate(AssessorService.Domain.Entities.FinancialGrade grade)
        {
            Grade = grade ?? new AssessorService.Domain.Entities.FinancialGrade();

            OutstandingFinancialDueDate = new FinancialDueDate();
            GoodFinancialDueDate = new FinancialDueDate();
            SatisfactoryFinancialDueDate = new FinancialDueDate();
            MonitoringFinancialDueDate = new FinancialDueDate();

            if (Grade.FinancialDueDate.HasValue)
            {
                var day = Grade.FinancialDueDate.Value.Day.ToString();
                var month = Grade.FinancialDueDate.Value.Month.ToString();
                var year = Grade.FinancialDueDate.Value.Year.ToString();

                switch (Grade.SelectedGrade)
                {
                    case AssessorService.Domain.Entities.FinancialApplicationSelectedGrade.Outstanding:
                        OutstandingFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    case AssessorService.Domain.Entities.FinancialApplicationSelectedGrade.Good:
                        GoodFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    case AssessorService.Domain.Entities.FinancialApplicationSelectedGrade.Satisfactory:
                        SatisfactoryFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    case AssessorService.Domain.Entities.FinancialApplicationSelectedGrade.Monitoring:
                        MonitoringFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class FinancialDueDate
    {
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public DateTime ToDateTime()
        {
            return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
        }
    }
}