using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.DTOs
{
    public class Gateway
    {
        public Guid ApplicationId { get; set; }
        public List<Outcome> Outcomes { get; set; }
    }
}
