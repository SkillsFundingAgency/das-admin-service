using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Financial
{
    public class RoatpFinancialApplicationViewModel : OrganisationDetailsViewModel
    {
        public List<Section> Sections { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; set; }

        public string DeclaredInApplication { get; set; }

        public FinancialReviewDetails FinancialReviewDetails { get; set; }

        public FinancialDueDate OutstandingFinancialDueDate { get; set; }
        public FinancialDueDate GoodFinancialDueDate { get; set; }
        public FinancialDueDate SatisfactoryFinancialDueDate { get; set; }

        public string InadequateComments { get; set; }
        public string ClarificationComments { get; set; }
        
        public string ClarificationResponse { get; set; }
        public string ApplicantEmailAddress { get; set; }

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

            SetupDeclaredInApplication(application.ApplyData);
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

            if(FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate)
            {
                InadequateComments = FinancialReviewDetails.Comments;
            }
            else if (FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Clarification)
            {
                ClarificationComments = FinancialReviewDetails.Comments;
            }
        }

        private void SetupDeclaredInApplication(RoatpApplyData applyData)
        {
            var fhaSequence = applyData?.Sequences.FirstOrDefault(seq => seq.SequenceNo == RoatpQnaConstants.RoatpSequences.FinancialEvidence);

            if (fhaSequence != null)
            {
                DeclaredInApplication = fhaSequence.NotRequired ? "Exempt" : "Not exempt";
            }
        }

        public string GetDownloadFilesLinkText(int sequenceNo, int sectionNo)
        {
            string linkText;

            if (sequenceNo == RoatpQnaConstants.RoatpSequences.FinancialEvidence && sectionNo == RoatpQnaConstants.RoatpSections.FinancialEvidence.YourOrganisationsFinancialEvidence)
            {
                linkText = "Download organisation's financial statements";
            }
            else if (sequenceNo == RoatpQnaConstants.RoatpSequences.FinancialEvidence && sectionNo == RoatpQnaConstants.RoatpSections.FinancialEvidence.YourUkUltimateParentCompanysFinancialEvidence)
            {
                linkText = "Download parent company's financial statements";
            }
            else
            {
                linkText = "Download financial statements";
            }
            
            return linkText;
        }
    }
}