using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class StandardOptions
    {
        public string StandardUId { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> CourseOption { get; set; }

        public bool HasMoreThanOneOption() => CourseOption != null && CourseOption.Count() > 1;
    }
}
