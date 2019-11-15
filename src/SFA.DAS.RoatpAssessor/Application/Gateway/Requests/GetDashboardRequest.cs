using MediatR;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetDashboardRequest : IRequest<GetDashboardResponse>
    {
        public DashboardTab Tab { get; }
        public bool SortDescending { get; }
        public string SortBy { get; }

        public GetDashboardRequest(DashboardTab tab, string sortBy, bool sortDescending)
        {
            Tab = tab;
            SortDescending = sortDescending;
            SortBy = sortBy;
        }
    }
}
