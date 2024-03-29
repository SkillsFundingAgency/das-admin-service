﻿using System;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.ViewModels
{
    public interface ICertificateViewModel
    {
        Guid Id { get; set; }
        string FamilyName { get; set; }
        string GivenNames { get; set; }
        void FromCertificate(Certificate cert);
        bool BackToCheckPage { get; set; }
        string ReasonForChange { get; set; }
        bool RequiresReasonForChange { get; set; }

        Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData certData);
    }
}