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

        public string VersionsToProcess { get; set; }

        public string CurrentVersion { get; set; }

        public WithdrawalDateCheckViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence,
            List<Section> sections, List<ApplySection> applySections, string backAction, string backController, string backOrganisationId,
            string versionsToProcess)
        : base(application, organisation, sequence, sections, applySections, backAction, backController, backOrganisationId)
        {
            RequestedWithdrawalDate = GetWithdrawalDate(Sections);
            VersionsToProcess = versionsToProcess;
            if(null == VersionsToProcess)
            {
                VersionsToProcess = string.Join(",", application.ApplyData.Apply.Versions);
            }
            PopNextVersion();
        }

        private void PopNextVersion()
        {
            CurrentVersion = null;
            if (!string.IsNullOrWhiteSpace(VersionsToProcess))
            {
                var versionList = VersionsToProcess.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                if(versionList.Any())
                {
                    CurrentVersion = versionList[0];
                    versionList.RemoveAt(0);
                    if(versionList.Any())
                    {
                        VersionsToProcess = string.Join(",", versionList);
                    }
                    else
                    {
                        VersionsToProcess = string.Empty;
                    }
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
