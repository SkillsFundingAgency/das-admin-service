using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateRequestReprintCommand : IRequest
    {
        public Guid CertificateId { get; set; }

        public string Username { get; set; }
    }
}