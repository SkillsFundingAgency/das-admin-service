using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateDelete
{
    public class CertificateConfirmDelete : CertificateBaseViewModel, ICertificateViewModel
    {
        public bool? IsSectionComplete { get; set; }
        public int StandardCode { get; set; }
        public long Uln { get; set; }
        public string SearchString { get; set; }

        public void FromCertificate(Certificate cert)
        {
            throw new NotImplementedException();
        }

        public Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData)
        {
            throw new NotImplementedException();
        }
    }
}
