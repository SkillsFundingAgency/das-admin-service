using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    public class RoatpFinancialApplicationViewModel : OrganisationDetailsViewModel
    {
        public List<Section> Sections { get; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; set; }

        public FinancialDueDate OutstandingFinancialDueDate { get; set; }
        public FinancialDueDate GoodFinancialDueDate { get; set; }
        public FinancialDueDate SatisfactoryFinancialDueDate { get; set; }
        public FinancialReviewDetails FinancialReviewDetails { get; set; }

        public RoatpFinancialApplicationViewModel() { }

        public RoatpFinancialApplicationViewModel(RoatpApplicationResponse application, Section parentCompanySection, Section activelyTradingSection, Section organisationTypeSection, List<Section> financialSections)
        {
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            Sections = SetupSections(parentCompanySection, activelyTradingSection, organisationTypeSection, financialSections);
            SetupGradeAndFinancialDueDate(application.FinancialGrade);

            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
        }

        private List<Section> SetupSections(Section parentCompanySection, Section activelyTradingSection, Section organisationTypeSection, List<Section> financialSections)
        {
            var sections = new List<Section>();

            if (parentCompanySection != null)
            {
                sections.Add(parentCompanySection);
            }

            if (activelyTradingSection != null)
            {
                sections.Add(activelyTradingSection);
            }

            if (organisationTypeSection != null)
            {
                sections.Add(organisationTypeSection);
            }

            if (financialSections != null)
            {
                sections.AddRange(financialSections);
            }

            return sections;
        }

        private void SetupGradeAndFinancialDueDate(FinancialReviewDetails financialReviewDetails)
        {
            FinancialReviewDetails = financialReviewDetails ?? new FinancialReviewDetails();

            OutstandingFinancialDueDate = new FinancialDueDate();
            GoodFinancialDueDate = new FinancialDueDate();
            SatisfactoryFinancialDueDate = new FinancialDueDate();

            if(FinancialReviewDetails.FinancialDueDate.HasValue)
            {
                var day = FinancialReviewDetails.FinancialDueDate.Value.Day.ToString();
                var month = FinancialReviewDetails.FinancialDueDate.Value.Month.ToString();
                var year = FinancialReviewDetails.FinancialDueDate.Value.Year.ToString();

                switch (FinancialReviewDetails.SelectedGrade)
                {
                    case FinancialApplicationSelectedGrade.Outstanding:
                        OutstandingFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    case FinancialApplicationSelectedGrade.Good:
                        GoodFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    case FinancialApplicationSelectedGrade.Satisfactory:
                        SatisfactoryFinancialDueDate = new FinancialDueDate { Day = day, Month = month, Year = year };
                        break;
                    default:
                        break;
                }
            }
        }
    }

}