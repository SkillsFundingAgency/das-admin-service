namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class SortOrderExtensions
    {
        public static Types.SortOrder Reverse(this Types.SortOrder sortOrder)
        {
            return sortOrder == Types.SortOrder.Ascending
                ? Types.SortOrder.Descending
                : Types.SortOrder.Ascending;
        }
    }
}
