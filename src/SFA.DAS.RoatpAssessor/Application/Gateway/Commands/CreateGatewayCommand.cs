using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class CreateGatewayCommand : IRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string UserName { get; }

        public CreateGatewayCommand(Guid applicationId, string userId, string userName)
        {
            ApplicationId = applicationId;
            UserId = userId;
            UserName = userName;
        }
    }
}
