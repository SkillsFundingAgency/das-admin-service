using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RoatpApplication = SFA.DAS.RoatpAssessor.Domain.DTOs.Application;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetDashboardHandler : IRequestHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly IApplyApiClient _applyApiClient;

        public GetDashboardHandler(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            var countsTask = _applyApiClient.GetGatewayCountsAsync();

            var newApplicationsTask = request.Tab == DashboardTab.New 
                ? _applyApiClient.GetSubmittedApplicationsAsync() 
                : Task.FromResult(new List<RoatpApplication>());

            var inProgressTask = request.Tab == DashboardTab.InProgress
                ? _applyApiClient.GetInProgressAsync()
                : Task.FromResult(new List<Domain.DTOs.Gateway>());

            await Task.WhenAll(countsTask, newApplicationsTask);

            var newApplications = SortApplications(newApplicationsTask.Result, request.SortBy, request.SortDescending);
            var inProgress = SortGateway(inProgressTask.Result, request.SortBy, request.SortDescending);

            var response = new GetDashboardResponse
            {
                Tab = request.Tab,
                NewApplicationsCount = countsTask.Result.NewApplications,
                InProgressCount = countsTask.Result.InProgress,
                NewApplications = newApplications,
                InProgress = inProgress,
                SortedBy = request.SortBy ?? nameof(RoatpApplication.SubmittedAt),
                SortedDescending = request.SortDescending
            };
           
            return response;
        }

        private List<RoatpApplication> SortApplications(List<RoatpApplication> applications, string sortBy, bool sortDescending)
        {
            if (applications == null)
                return null;

            switch (sortBy)
            {
                case nameof(RoatpApplication.OrganisationName):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.OrganisationName).ToList()
                        : applications.OrderBy(a => a.OrganisationName).ToList();

                case nameof(RoatpApplication.Ukprn):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.Ukprn).ToList()
                        : applications.OrderBy(a => a.Ukprn).ToList();

                case nameof(RoatpApplication.ApplicationRef):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.ApplicationRef).ToList()
                        : applications.OrderBy(a => a.ApplicationRef).ToList();

                case nameof(RoatpApplication.ProviderRoute):
                    return sortDescending
                        ? applications.OrderByDescending(a => a.ProviderRoute).ToList()
                        : applications.OrderBy(a => a.ProviderRoute).ToList();

                case nameof(RoatpApplication.SubmittedAt):
                default:
                    return sortDescending
                        ? applications.OrderByDescending(a => a.SubmittedAt).ToList()
                        : applications.OrderBy(a => a.SubmittedAt).ToList();
            }
        }

        private List<Domain.DTOs.Gateway> SortGateway(List<Domain.DTOs.Gateway> reviews, string sortBy, bool sortDescending)
        {
            if (reviews == null)
                return null;

            switch (sortBy)
            {
                case nameof(Domain.DTOs.Gateway.OrganisationName):
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.OrganisationName).ToList()
                        : reviews.OrderBy(a => a.OrganisationName).ToList();

                case nameof(Domain.DTOs.Gateway.Ukprn):
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.Ukprn).ToList()
                        : reviews.OrderBy(a => a.Ukprn).ToList();

                case nameof(Domain.DTOs.Gateway.ApplicationRef):
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.ApplicationRef).ToList()
                        : reviews.OrderBy(a => a.ApplicationRef).ToList();

                case nameof(Domain.DTOs.Gateway.ProviderRoute):
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.ProviderRoute).ToList()
                        : reviews.OrderBy(a => a.ProviderRoute).ToList();

                case nameof(Domain.DTOs.Gateway.AssignedToName):
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.AssignedToName).ToList()
                        : reviews.OrderBy(a => a.AssignedToName).ToList();

                case nameof(Domain.DTOs.Gateway.SubmittedAt):
                default:
                    return sortDescending
                        ? reviews.OrderByDescending(a => a.SubmittedAt).ToList()
                        : reviews.OrderBy(a => a.SubmittedAt).ToList();

            }
        }
    }
}
