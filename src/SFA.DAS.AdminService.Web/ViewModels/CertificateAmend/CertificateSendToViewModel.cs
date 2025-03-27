using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Entities;


namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateSendToViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public CertificateSendTo SendTo { get; set; }

        public CertificateSendToViewModel()
        {
            RequiresReasonForChange = false;
        }

        public override void FromCertificate(Certificate certificate)
        {
            base.FromCertificate(certificate);

            SendTo = CertificateData.SendTo;
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            certData.SendTo = SendTo;
            certificate.CertificateData = certData;
            
            return certificate;
        }

        public bool SendToHasChanged(CertificateData certData)
        {
            return certData.SendTo != SendTo;
        }
    }
}