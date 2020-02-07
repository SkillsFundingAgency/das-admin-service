using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpSectionViewModel : OrganisationDetailsViewModel
    {
        public FinancialReviewDetails Grade { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public string CompanyNumber { get; set; }

        public string Title { get; }
        public string Status { get; set; }

        public Section Section { get; }
        public RoatpApplySection ApplySection { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public int SectionNo { get; }

        public bool? IsSectionComplete { get; set; }
        public Dictionary<string, AddressViewModel> Addresses = new Dictionary<string, AddressViewModel>();
        public RoatpSectionViewModel(RoatpApplicationResponse application, Organisation organisation, Section section, RoatpApplySection applySection)
        {
            ApplicationId = application.ApplicationId;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRoute;
            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;
            Grade = application.FinancialGrade;

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

            foreach (var pg in Section.QnAData.Pages)
            {
                foreach (var answerPage in pg.PageOfAnswers)
                {
                    foreach (var answer in answerPage.Answers)
                    {
                        var question = pg.Questions.SingleOrDefault(q => q.QuestionId == answer.QuestionId);
                        if (question != null && question.Input.Type == "Address")
                        {
                            Addresses.Add(answer.QuestionId, JsonConvert.DeserializeObject<AddressViewModel>(answer.Value));
                        }
                    }
                }
            }
        }

        public string DisplayAnswerValue(QnA.Api.Types.Page.Answer answer, QnA.Api.Types.Page.Question question)
        {
            if (question?.Input?.Type == "Date" || question?.Input?.Type == "MonthAndYear")
            {
                var dateparts = answer.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (question.Input.Type == "Date")
                {
                    var datetime = DateTime.Parse($"{dateparts[0]}/{dateparts[1]}/{dateparts[2]}");
                    return datetime.ToString("dd/MM/yyyy");
                }
                else if (question.Input.Type == "MonthAndYear")
                {
                    DateTime datetime;
                    DateTime.TryParse($"{dateparts[0]}/{dateparts[1]}", out datetime);
                    return datetime.ToString("MM/yyyy");
                }
            }

            return answer.Value;
        }
    }
}
