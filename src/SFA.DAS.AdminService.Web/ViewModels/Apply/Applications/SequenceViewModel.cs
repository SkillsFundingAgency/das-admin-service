using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SequenceViewModel : BackViewModel
    {
        public SequenceViewModel(ApplicationResponse application, Organisation organisation, Sequence sequence, 
            List<Section> sections, List<ApplySection> applySections, string backAction, string backController, string backOrganisationId,
            List<ApplicationResponse> previousWithdrawal = null)
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

            PreviousWithdrawal = GetPreviousWithdrawal(previousWithdrawal, application);
        }

        private ApplicationResponse GetPreviousWithdrawal(List<ApplicationResponse> previousWithdrawals, ApplicationResponse application)
        {
            //Version applications
            if (application.StandardApplicationType == "version") 
            {
                foreach (var withdrawal in previousWithdrawals)
                {
                    if (withdrawal.ApplyData.Apply.Versions == application.ApplyData.Apply.Versions)
                    {
                        return withdrawal;
                    }
                }
            }
            else //Standard applications
            {
                foreach (var withdrawal in previousWithdrawals)
                {
                    if (withdrawal.ApplicationType == "StandardWithdrawal")
                    {
                        return withdrawal;
                    }
                }
            }
            
            

            return null;
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

        ApplicationResponse PreviousWithdrawal { get; set; }

        public string PreviousWithdrawalType => PreviousWithdrawal?.StandardApplicationType.Replace("Withdrawal","");
        public DateTime? PreviousWithdrawalDate => PreviousWithdrawal?.ApplyData.Sequences.Where(x => x.SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO).Select(y => y.ApprovedDate).FirstOrDefault();
        public bool? IsPreviousWithdrawalOlderThanTwelveMonths => PreviousWithdrawalDate != null && PreviousWithdrawalDate < DateTime.UtcNow.AddMonths(-12);


        public bool IsWithdrawal => SequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO ||
                                    SequenceNo == ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;        

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
