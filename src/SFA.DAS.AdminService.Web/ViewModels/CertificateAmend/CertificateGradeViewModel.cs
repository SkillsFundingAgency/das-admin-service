using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateGradeViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public string SelectedGrade { get; set; }

        public List<SelectListItem> Grades = new List<SelectListItem>
        {
            new SelectListItem {Text = "Pass", Value = "Pass"},
            new SelectListItem {Text = "Credit", Value = "Credit"},
            new SelectListItem {Text = "Merit", Value = "Merit"},
            new SelectListItem {Text = "Distinction", Value = "Distinction"},
            new SelectListItem {Text = "Pass with excellence", Value = "Pass with excellence"},
            new SelectListItem {Text = "Outstanding", Value = "Outstanding"},
            new SelectListItem {Text = "No grade awarded", Value = "No grade awarded"}
        };
        
        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);
            
            SelectedGrade = CertificateData.OverallGrade;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.OverallGrade = SelectedGrade;
            certificate.CertificateData = JsonSerializer.Serialize(data);
            
            return certificate;
        }
    }
}