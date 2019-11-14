
using Newtonsoft.Json;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class PageViewModel : BackViewModel
    {
        public Page Page { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceNo { get; }

        public int SectionNo { get; }

        public string PageId { get; }

        public string FeedbackMessage { get; set; }

        public Dictionary<string, AddressViewModel> Addresses = new Dictionary<string, AddressViewModel>();

        public PageViewModel(Guid applicationId, int sequenceNo, int sectionNo, string pageId,Section section, Page page, string backAction, string backController, string backOrganisationId)
            : base(backAction, backController, backOrganisationId)
        {
            if (page != null)
            {
                Page = page;
                Title = page.Title;
                ApplicationId = applicationId;
                SequenceNo = sequenceNo;
                SectionNo = sectionNo;
                PageId = page.PageId;

                foreach (var pg in section.QnAData.Pages)
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
            else
            {
                ApplicationId = applicationId;
                SequenceNo = sequenceNo;
                SectionNo = sectionNo;
                PageId = pageId;
            }
        }
    }
}
