using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpTaskListViewModel
    {
        public RoatpTaskListViewModel(RoatpApplicationResponse application, Organisation organisation, List<Sequence> sequences, 
                                      List<RoatpApplySequence> applySequences, List<RoatpSequence> roatpSequences)
        {
            ApplicationId = application.ApplicationId;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;

            FinancialReviewStatus = application.FinancialReviewStatus;
            FinancialDueDate = application.financialGrade?.FinancialDueDate;    
            
            foreach(var sequence in applySequences)
            {
                var roatpSequence = roatpSequences.FirstOrDefault(x => x.Id == sequence.SequenceNo);
                if (roatpSequence != null)
                {
                    sequence.Description = roatpSequence.Title;
                }
                if (roatpSequence == null || !roatpSequence.Roles.Contains(Roles.RoatpAssessorTeam))
                {
                    sequence.NotRequired = true;
                }
            }

            ApplySequences = applySequences.Where(x => !x.NotRequired).ToList();
            Sequences = sequences;
        }

        private List<RoatpApplySection> GetRequiredApplySections(List<RoatpApplySection> applySections)
        {
            return applySections.Where(s => !s.NotRequired).ToList();
        }

        private List<Section> GetRequiredSections(List<RoatpApplySection> applySections, List<Section> sections)
        {
            var requiredSectionsNos = applySections.Where(s => !s.NotRequired).Select(s => s.SectionNo).ToList();

            return sections.Where(s => requiredSectionsNos.Contains(s.SectionNo)).ToList();
        }

        public string ApplicationReference { get; set; }
        public string FinancialReviewStatus { get; set; }
        public DateTime? FinancialDueDate { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public string CompanyNumber { get; set; }

        public string Status { get; set; }
        public List<Sequence> Sequences { get; }
        public List<RoatpApplySequence> ApplySequences { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
    }
}
