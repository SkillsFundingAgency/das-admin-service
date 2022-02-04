namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class StringExtensions
    {
        public static string CapitaliseFirstLetter(this string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1); 
        }
    }
}
