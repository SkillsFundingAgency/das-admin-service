using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Apply
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string CreatedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string StandardApplicationType { get; set; }

        public List<string> Versions => ApplyData?.Apply?.Versions;
        public ApplySequence ActiveSequence => ApplyData.Sequences.Where(seq => seq.IsActive && !seq.NotRequired).OrderBy(seq => seq.SequenceNo).FirstOrDefault();
        public bool RequiresFinancialApproval => ApplyData?.Sequences.FirstOrDefault(seq => seq.SequenceNo == ApplyConst.ORGANISATION_SEQUENCE_NO)?.NotRequired == false;

        public string GetStandardDescriptionWithVersions(List<string> versions, int sequenceNo)
        {
            var standardDescription = ApplyData?.Apply?.StandardWithReference;

            if (sequenceNo == ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO && versions != null && versions.Any())
            {
                standardDescription += $", Version {string.Join(",", versions)}";
            }

            return standardDescription;
        }
    }
}
