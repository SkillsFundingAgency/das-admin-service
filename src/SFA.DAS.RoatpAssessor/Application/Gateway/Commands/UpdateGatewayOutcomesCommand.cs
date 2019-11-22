using MediatR;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class UpdateGatewayOutcomesCommand : IRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public DateTime ChangedAt { get; }
        public List<Outcome> OutcomesDelta { get; }

        public UpdateGatewayOutcomesCommand(Guid applicationId, string userId, DateTime changedAt, Outcome outcomeDelta)
            : this(applicationId, userId, changedAt, new List<Outcome> { outcomeDelta }) { }

        public UpdateGatewayOutcomesCommand(Guid applicationId, string userId, DateTime changedAt, List<Outcome> outcomesDelta)
        {
            ApplicationId = applicationId;
            UserId = userId;
            ChangedAt = changedAt;
            OutcomesDelta = outcomesDelta;
        }
    }
}
