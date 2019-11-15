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
            var countsTask = _applyApiClient.GetGatewayCounts();

            var newApplicationsTask = request.Tab == DashboardTab.New 
                ? _applyApiClient.GetSubmittedApplicationsAsync() 
                : Task.FromResult(new List<RoatpApplication>());

            await Task.WhenAll(countsTask, newApplicationsTask);

            var newApplications = SortApplications(newApplicationsTask.Result, request.SortBy, request.SortDescending);

            var response = new GetDashboardResponse
            {
                Tab = request.Tab,
                NewApplicationsCount = countsTask.Result.NewApplications,
                InProgressCount = countsTask.Result.InProgress,
                NewApplications = newApplications,
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
    }
}
