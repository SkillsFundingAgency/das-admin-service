using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationSequenceAssessmentViewModel : BackViewModel
    {
        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public List<Section> Sections { get; }

        public bool HasNewFeedback { get; }
        public bool HasInadequateFhaButNoFeedbackGiven { get; }

        public string ReturnType { get; set; }

        public ApplicationSequenceAssessmentViewModel(ApplicationResponse application, Sequence sequence, List<Section> sections, string backAction, string backController, string backOrganisationId)
            : base (backAction, backController, backOrganisationId)
        {
            ApplicationId = application.Id;
            SequenceNo = sequence.SequenceNo;
            Sections = sections;

            HasNewFeedback = sections.Any(sec => sec.QnAData.RequestedFeedbackAnswered.HasValue && !sec.QnAData.RequestedFeedbackAnswered.Value);

            var fhaSection = sections.FirstOrDefault(s => s.SectionNo == 3);

            if(fhaSection != null && !fhaSection.QnAData.Pages.Any(p => p.HasNewFeedback))
            {
                HasInadequateFhaButNoFeedbackGiven = application.FinancialGrade?.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate;
            }
        }
    }
}
