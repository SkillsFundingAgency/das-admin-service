﻿using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class WithdrawStandardRequest : IRequest
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime WithdrawalDate { get; set; }
    }
}
