using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Configuration
{
    public class GatewayPageConfiguration
    {

        public string PageId { get; set; }
        public string NextPageId { get; set; }

        public string Caption { get; set; }
        public string Heading { get; set; }
        public List<TextListingDetail> TextListings { get; set; }
    }

    public class TextListingDetail
    {
        public List<TextListingElement> Elements { get; set; }
    }

    public class TextListingElement
    {
        public string Source { get; set; }   //"Text", "ApplyQuestionTag", "ApplySpecial" (eg submitted on date, sources checked on)
        public string Value { get; set; }
        public string Key { get; set; }     // When ApplyQuestionTag, this contains the tag
    }

   
}
