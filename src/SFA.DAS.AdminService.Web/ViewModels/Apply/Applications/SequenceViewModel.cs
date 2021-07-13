using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SequenceViewModel : BackViewModel
    {
        public SequenceViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence, 
            List<Section> sections, List<ApplySection> applySections, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            ApplicationId = application.Id;
            ApplicationReference = application.ApplyData.Apply.ReferenceNumber;
            StandardName = application.ApplyData.Apply.StandardName;
            StandardCode = application.ApplyData.Apply.StandardCode;
            StandardReference = application.ApplyData.Apply.StandardReference;
            StandardVersion = (null != application.ApplyData.Apply.Versions) ? string.Join(",",application.ApplyData.Apply.Versions) : null;
            ReviewStatus = application.ReviewStatus;

            FinancialReviewStatus = application.FinancialReviewStatus;
            FinancialDueDate = application.FinancialGrade?.FinancialDueDate;

            LegalName = organisation.OrganisationData.LegalName;
            TradingName = organisation.OrganisationData.TradingName;
            ProviderName = organisation.OrganisationData.ProviderName;
            Ukprn = organisation.EndPointAssessorUkprn;
            CompanyNumber = organisation.OrganisationData.CompanyNumber;
            OrganisationName = organisation.EndPointAssessorName;

            ApplySections = GetRequiredApplySections(applySections);
            Sections = GetRequiredSections(applySections, sections);
            
            SequenceNo = sequence.SequenceNo;
            Status = sequence.Status;

            ContactName = application.ContactName;
            ContactEmail = application.ContactEmail;

            WithdrawalDate = GetWithdrawalDate(Sections);
        }

        private List<ApplySection> GetRequiredApplySections(List<ApplySection> applySections)
        {
            return applySections.Where(s => !s.NotRequired).ToList();
        }

        private List<Section> GetRequiredSections(List<ApplySection> applySections, List<Section> sections)
        {
            var requiredSectionsNos = applySections.Where(s => !s.NotRequired).Select(s => s.SectionNo).ToList();

            return sections.Where(s => requiredSectionsNos.Contains(s.SectionNo)).ToList();
        }

        private DateTime? GetWithdrawalDate(List<Section> sections)
        {
            DateTime? withdrawalDate = default(DateTime?);

            if(null != sections)
            {
                var withdrawalSection = sections.FirstOrDefault(s => s.SequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO || s.SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO);
                if(null != withdrawalSection)
                {
                    var withdrawalDatePage = withdrawalSection.QnAData.Pages.FirstOrDefault(p => p.LinkTitle.Trim().ToUpper() == "WITHDRAWAL DATE");  //@ToDo: tech debt - very brittle
                    if(null != withdrawalDatePage && withdrawalDatePage.PageOfAnswers.Any())
                    {
                        var answers = withdrawalDatePage.PageOfAnswers[0].Answers;
                        if(answers.Any())
                        {
                            var answer = answers[0];
                            withdrawalDate = DateTime.Parse(answer.Value);
                        }
                    }
                }
            }

            return withdrawalDate;
        }

        public string ApplicationReference { get; set; }
        public string StandardName { get; set; }
        public string StandardReference { get; set; }
        public int? StandardCode { get; set; }
        public string Standard => StandardCode.HasValue ? $"{StandardName} ({StandardCode})" : StandardName;
        public string StandardWithReference => $"{StandardName} ({StandardReference})";
        public string StandardVersion { get; set; }
        public string ReviewStatus { get; set; }

        public string FinancialReviewStatus { get; set; }
        public DateTime? FinancialDueDate { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public string CompanyNumber { get; set; }
        public string OrganisationName { get; }

        public string Status { get; set; }
        public List<Section> Sections { get; }
        public List<ApplySection> ApplySections { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }

        public bool IsWithdrawal => SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO ||
                                    SequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
        public DateTime? WithdrawalDate { get; set; }

        public string ContactName { get; }
        public string ContactEmail { get; }

        // SV-657 helper to set the link text.
        // SV-914 Withdrawal application
        public string GetApplicationLinkText(string linkTitle, bool isWithdrawal = false)
        {
            var text = $"Evaluate {linkTitle.ToLower()}";
            if(IsWithdrawal)
            {
                text = "Evaluate withdrawal application";
            }
            return text;
        }
    }
}
