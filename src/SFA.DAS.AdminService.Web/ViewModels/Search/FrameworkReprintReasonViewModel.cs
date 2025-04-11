using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessorService.Api.Types.Enums;

namespace SFA.DAS.AdminService.Web.ViewModels.Search
{
    public class FrameworkReprintReasonViewModel
    {
        public string CertificateReference { get; set; }
        public string ApprenticeName { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? CertificatePrintStatusAt { get; set; }

        public string TicketNumber { get; set; }
        public List<string> SelectedReprintReasons { get; set; }
        public string OtherReason { get; set; }
        public bool BackToCheckAnswers { get; set; }
        public string BackAction { get; set; }

        public static string GetReprintReasonDescription(ReprintReasons reprintReason)
        {
            return reprintReason.AsString(EnumFormat.Description);
        }

        public string ReprintReasonDisplay
        {
            get
            {
                if (SelectedReprintReasons.IsNullOrEmpty())
                {
                    return string.Empty;
                }
                else if (SelectedReprintReasons.Count == 1)
                {
                    if (SelectedReprintReasons.Contains("Other"))
                    {
                        return OtherReason;
                    }
                    else
                    {
                        return SelectedReprintReasons[0];
                    }
                }
                
                var displayReasons = SelectedReprintReasons.Select(reason => reason == "Other" ? OtherReason : reason).ToList();
                return $"<ul class=\"govuk-list govuk-list--bullet\"><li>{string.Join("</li><li>", displayReasons)}</li></ul>";
            }
        }

        public bool IsSingleReason
        {
            get
            {
                return SelectedReprintReasons != null && SelectedReprintReasons.Count == 1;
            }
        }
    }
}
