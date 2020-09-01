using MediatR;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEMailTemplateRequest : IRequest<EmailTemplateSummary>
    {
        public string TemplateName { get; set; }
    }
}
