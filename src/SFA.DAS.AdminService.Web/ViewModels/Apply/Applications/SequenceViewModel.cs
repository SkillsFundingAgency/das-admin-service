using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class SequenceViewModel
    {
        public SequenceViewModel(Sequence sequence, Guid Id, List<Section> sections, List<ApplySection> applySection)
        {
            this.Id = Id;
            Sections = sections;
            ApplySections = applySection;
            SequenceNo = sequence.SequenceNo;
            Status = sequence.Status;
        }

        public string Status { get; set; }
        public List<Section> Sections { get; }
        public List<ApplySection> ApplySections { get; }
        public Guid Id { get; }
        public int SequenceNo { get; }
    }
}
