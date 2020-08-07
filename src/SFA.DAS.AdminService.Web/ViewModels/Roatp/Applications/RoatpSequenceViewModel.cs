﻿using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class RoatpSequenceViewModel : OrganisationDetailsViewModel
    {
        public RoatpSequenceViewModel(RoatpApplicationResponse application, Organisation organisation, 
            Sequence sequence, List<Section> sections, List<RoatpApplySection> applySections,
            List<RoatpSequence> roatpSequences)
        {
            ApplicationId = application.ApplicationId;
            ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
            ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
            SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            Ukprn = application.ApplyData.ApplyDetails.UKPRN;

            FinancialReviewStatus = application.FinancialReviewStatus;
            FinancialDueDate = application.FinancialGrade?.FinancialDueDate;

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

            var requiredSections = sections.Where(s => requiredSectionsNos.Contains(s.SectionNo)).ToList();
            foreach(var requiredSection in requiredSections)
            {
                var applySection = applySections.FirstOrDefault(s => s.SectionNo == requiredSection.SectionNo);
                requiredSection.Status = applySection.Status;
            }

            return requiredSections;
        }
              
        
        public string FinancialReviewStatus { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        
        public string Status { get; set; }
        public List<Section> Sections { get; }
        public List<RoatpApplySection> ApplySections { get; }
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
    }
}
