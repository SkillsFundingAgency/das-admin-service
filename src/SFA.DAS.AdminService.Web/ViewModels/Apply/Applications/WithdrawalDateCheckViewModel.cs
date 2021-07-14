using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
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

        // If the withdrawal is a standard withdrawal for multiple versions of the standard,
        // this holds the specific version being processed by the withdrawal date check.
        // Otherwise null.
        public string SpecifiedVersion { get; set; }

        // Mapping from version to withdrawal date
        public Dictionary<string, DateTime?> StandardVersionWithdrawalDates { get; set; }

        public WithdrawalDateCheckViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence,
            List<Section> sections, List<ApplySection> applySections, string backAction, string backController, string backOrganisationId,
            string specifiedVersion, Dictionary<string, DateTime?> standardVersionWithdrawalDates)
        : base(application, organisation, sequence, sections, applySections, backAction, backController, backOrganisationId)
        {
            RequestedWithdrawalDate = GetWithdrawalDate(Sections);

            if(null != standardVersionWithdrawalDates && standardVersionWithdrawalDates.Any())
            {
                StandardVersionWithdrawalDates = standardVersionWithdrawalDates;
            }
            else
            {
                InitStandardVersionWithdrawalDates();
            }

            if(!string.IsNullOrWhiteSpace(specifiedVersion))
            {
                SpecifiedVersion = specifiedVersion;
            }
            else
            {
                if(null != StandardVersionWithdrawalDates && StandardVersionWithdrawalDates.Any())
                {
                    SpecifiedVersion = StandardVersionWithdrawalDates.First().Key;
                }
            }
        }
        
        private void InitStandardVersionWithdrawalDates()
        {
            var requestWithdrawalDate = GetWithdrawalDate(Sections);

            if (!string.IsNullOrWhiteSpace(StandardVersion))
            {
                var versionList = StandardVersion.Split(",", StringSplitOptions.RemoveEmptyEntries);
                StandardVersionWithdrawalDates = new Dictionary<string, DateTime?>();
                foreach(var version in versionList)
                {
                    StandardVersionWithdrawalDates.Add(version, RequestedWithdrawalDate);
                }
            }
        }

        private DateTime? GetWithdrawalDate(List<Section> sections)
        {
            DateTime? withdrawalDate = default(DateTime?);

            if (null != sections)
            {
                var withdrawalSection = sections.FirstOrDefault(s => s.SequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO || s.SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO);
                if (null != withdrawalSection)
                {
                    var withdrawalDatePage = withdrawalSection.QnAData.Pages.FirstOrDefault(p => p.LinkTitle.Trim().ToUpper() == "WITHDRAWAL DATE");  //@ToDo: tech debt - very brittle
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
