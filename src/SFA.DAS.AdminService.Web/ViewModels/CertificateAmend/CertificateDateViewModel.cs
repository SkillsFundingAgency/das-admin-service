﻿using System;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.ViewModels.CertificateAmend
{
    public class CertificateDateViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime? StartDate { get; set; }
        public string WarningShown { get; set; }

        public override void FromCertificate(Certificate cert)
        {
            base.FromCertificate(cert);

            Day = CertificateData.AchievementDate?.Day.ToString();
            Month = CertificateData.AchievementDate?.Month.ToString();
            Year = CertificateData.AchievementDate?.Year.ToString();
            StartDate = CertificateData.LearningStartDate;
           
            WarningShown = "false";
        }

        public override Certificate GetCertificateFromViewModel(Certificate certificate, CertificateData data)
        {
            data.AchievementDate = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));

            certificate.CertificateData = data;

            return certificate;
        }
    }
}