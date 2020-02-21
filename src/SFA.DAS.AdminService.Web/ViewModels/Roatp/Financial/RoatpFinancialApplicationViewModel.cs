using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
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

        public RoatpFinancialApplicationViewModel(RoatpApplicationResponse application, List<Section> sections)
        {
            if (sections != null && sections.Any())
            {
                Sections = sections;
                ApplicationId = application.ApplicationId;
            }
            else
            {
                ApplicationId = application.ApplicationId;
            }

            OrgId = application.OrganisationId;
            SetupGradeAndFinancialDueDate(application.FinancialGrade);

            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
        }

        private void SetupGradeAndFinancialDueDate(FinancialReviewDetails financialReviewDetails)
        {
            if (financialReviewDetails == null)
            {
                financialReviewDetails = new FinancialReviewDetails();
            }

            FinancialReviewDetails = financialReviewDetails;

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