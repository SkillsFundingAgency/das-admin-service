using MediatR;
using System;

namespace SFA.DAS.AdminService.Web.Commands.ReturnApplicationSequence
{
    public class ReturnApplicationSequenceCommand : IRequest
    {
        public Guid Id { get; set; }
        public int SequenceNo { get; set; }
        public string ReturnType { get; set; }
        public string Username { get; set; }

        public ReturnApplicationSequenceCommand(Guid id, int sequenceNo, string returnType, string username)
        {
            Id = id;
            SequenceNo = sequenceNo;
            ReturnType = returnType;
            Username = username;
        }
    }
}
