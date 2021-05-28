using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Standards
{
    public class StandardVersion
    {
        public string StandardUId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string IFateReferenceNumber { get; set; }
        public int LarsCode { get; set; }
        public int Level { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public IEnumerable<string> Options { get; set; }

    }
}
