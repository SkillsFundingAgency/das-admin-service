using MediatR;
using System;
using ApplicationResponse = SFA.DAS.AdminService.Web.Models.Apply.Application;

namespace SFA.DAS.AdminService.Web.Queries.GetApplication
{
    public class GetApplicationQuery : IRequest<ApplicationResponse>
    {
        public Guid Id { get; set; }

        public GetApplicationQuery(Guid id)
        {
            Id = id;
        }
    }
}
