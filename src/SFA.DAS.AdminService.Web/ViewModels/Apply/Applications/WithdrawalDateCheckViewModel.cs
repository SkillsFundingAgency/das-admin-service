using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class WithdrawalDateCheckViewModel : SequenceViewModel
    {
        public DateTime? RequestedWithdrawalDate { get; set; }

        public WithdrawalDateCheckViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence,
            List<Section> sections, List<ApplySection> applySections, string backAction, string backController, string backOrganisationId)
        : base(application, organisation, sequence, sections, applySections, backAction, backController, backOrganisationId, null)
        {
            RequestedWithdrawalDate = GetWithdrawalDate(Sections);
        }

        private DateTime? GetWithdrawalDate(List<Section> sections)
        {
            DateTime? withdrawalDate = default;

            if (null != sections)
            {
                var withdrawalSection = sections.FirstOrDefault(s => s.SequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO || s.SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO);
                if (null != withdrawalSection)
                {
                    var withdrawalDatePage = withdrawalSection.QnAData.Pages.FirstOrDefault(p => p.LinkTitle.Trim().ToUpper() == "WITHDRAWAL DATE");  // Reported in SV-1029 tech debt
                    if (null != withdrawalDatePage && withdrawalDatePage.PageOfAnswers.Any())
                    {
                        var answers = withdrawalDatePage.PageOfAnswers[0].Answers;
                        if (answers.Any())
                        {
                            var answer = answers[0];
                            withdrawalDate = DateTime.Parse(answer.Value);
                        }
                    }
                }
            }

            return withdrawalDate;
        }
    }
}
