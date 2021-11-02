using SFA.DAS.QnA.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Queries.GetSequenceAndSections
{
    public class GetSequenceAndSectionsResponse
    {
        public Sequence Sequence { get; set; }
        public List<Section> Sections { get; set; }
    }
}
