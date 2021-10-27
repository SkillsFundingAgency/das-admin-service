using MediatR;
using System;

namespace SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication
{
    public class ApproveStandardApplicationCommand : IRequest<ApproveStandardApplicationResponse>
    {
        public Guid ApplicationId { get; set; }
        public string ReturnType { get; set; }
        public int SequenceNo { get; set; }

        public ApproveStandardApplicationCommand(Guid applicationId, string returnType, int sequenceNo)
        {
            ApplicationId = applicationId;
            ReturnType = returnType;
            SequenceNo = sequenceNo;
        }
    }
}
