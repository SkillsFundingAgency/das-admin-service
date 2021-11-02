using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication
{
    public class ApproveStandardApplicationValidator : AbstractValidator<ApproveStandardApplicationCommand>
    {
        public ApproveStandardApplicationValidator()
        {
            RuleFor(c => c.ReturnType).NotNull().WithMessage("Must provide return type");
        }
    }
}
