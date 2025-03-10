using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class AmendFrameworkReprintReasonViewModel
    {
        public List<string> SelectedReprintReasons { get; set; }
        public string TicketNumber { get; set; }
        public string OtherReason { get; set; }
        public string ReprintReasonDisplay
        {
            get
            {
                if (SelectedReprintReasons == null || !SelectedReprintReasons.Any())
                {
                    return string.Empty;
                }

                if (SelectedReprintReasons.Count == 1)
                {
                    if (SelectedReprintReasons.Contains("Other"))
                    {
                        return OtherReason;
                    }
                    else
                    {
                        return SelectedReprintReasons.First();
                    }
                }
                else
                {
                    var displayReasons = SelectedReprintReasons.Select(reason => reason == "Other" ? OtherReason : reason).ToList();
                    return $"<ul class=\"govuk-list govuk-list--bullet\"><li>{string.Join("</li><li>", displayReasons)}</li></ul>";
                }
            }
        }
        public bool IsSingleReason
        {
            get{
                return SelectedReprintReasons != null && SelectedReprintReasons.Count == 1;
            }
        }
    }
}
