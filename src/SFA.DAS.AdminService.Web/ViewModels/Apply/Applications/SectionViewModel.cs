using Newtonsoft.Json;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SectionViewModel : BackViewModel
    {
        public string ApplicationReference { get; set; }
        public AssessorService.ApplyTypes.FinancialGrade Grade { get; set; }

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
        
        public Dictionary<string, AddressViewModel> Addresses = new Dictionary<string, AddressViewModel>();

        public SectionViewModel(ApplicationResponse application, Organisation organisation, Section section, ApplySection applySection, 
            Dictionary<string, object> applicationData, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            ApplicationId = application.Id;
            ApplicationReference = application.ApplyData.Apply.ReferenceNumber;
            Grade = application.FinancialGrade;

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

                foreach(var question in pg.Questions)
                {
                    question.Label = ApplicationDataFormatHelper.FormatApplicationDataPropertyPlaceholders(question.Label, applicationData);
                    question.Hint = ApplicationDataFormatHelper.FormatApplicationDataPropertyPlaceholders(question.Hint, applicationData);
                    question.QuestionBodyText = ApplicationDataFormatHelper.FormatApplicationDataPropertyPlaceholders(question.QuestionBodyText, applicationData);
                    question.ShortLabel = ApplicationDataFormatHelper.FormatApplicationDataPropertyPlaceholders(question.ShortLabel, applicationData);
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
