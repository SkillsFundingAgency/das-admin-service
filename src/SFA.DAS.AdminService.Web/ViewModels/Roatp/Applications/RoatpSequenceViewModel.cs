using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpSequenceViewModel
    {
        public RoatpSequenceViewModel(RoatpApplicationResponse application, Organisation organisation, 
            Sequence sequence, List<Section> sections, List<RoatpApplySection> applySections,
            List<RoatpSequence> roatpSequences)
        {
            ApplicationId = application.ApplicationId;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;

            FinancialReviewStatus = application.FinancialReviewStatus;
            FinancialDueDate = application.financialGrade?.FinancialDueDate;

            //LegalName = organisation.OrganisationData.LegalName;
            //TradingName = organisation.OrganisationData.TradingName;
            //ProviderName = organisation.OrganisationData.ProviderName;
            //Ukprn = organisation.EndPointAssessorUkprn;
            //CompanyNumber = organisation.OrganisationData.CompanyNumber;

            var roatpSequence = roatpSequences.FirstOrDefault(x => x.Id == sequence.SequenceNo);
            if (roatpSequence != null)
            {
                foreach (var excludedSectionId in roatpSequence.ExcludeSections)
                {
                    var section = applySections.FirstOrDefault(x => x.SectionNo.ToString() == excludedSectionId);
                    if (section != null)
                    {
                        section.NotRequired = true;
                    }                    
                }
            }           

            ApplySections = GetRequiredApplySections(applySections);
            Sections = GetRequiredSections(applySections, sections);
            
            SequenceNo = sequence.SequenceNo;
            Status = sequence.Status;
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
        public string StandardName { get; set; }
        public int? StandardCode { get; set; }
        public string Standard => StandardCode.HasValue ? $"{StandardName} ({StandardCode})" : StandardName;

        public string FinancialReviewStatus { get; set; }
        public DateTime? FinancialDueDate { get; set; }

        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string ProviderName { get; set; }
        public int? Ukprn { get; set; }
        public string CompanyNumber { get; set; }

        public string Status { get; set; }
        public List<Section> Sections { get; }
        public List<RoatpApplySection> ApplySections { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
    }
}
