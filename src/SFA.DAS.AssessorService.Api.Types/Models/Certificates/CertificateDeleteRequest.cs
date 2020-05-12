using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateDeleteRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string Username { get; set; }
        public string ReasonForChange { get; set; }
        public string  IncidentNumber { get; set; }
    }
}   