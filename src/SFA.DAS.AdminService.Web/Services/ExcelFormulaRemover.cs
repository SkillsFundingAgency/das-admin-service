namespace SFA.DAS.AdminService.Web.Services
{
    public static class ExcelFormulaRemover
    {
        public static string StripFormulae(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            var text = inputText;

            text = text.Replace("=", string.Empty);

            return text;
        }
    }
}
