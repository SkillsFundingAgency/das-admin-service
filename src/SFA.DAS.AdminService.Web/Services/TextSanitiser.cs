namespace SFA.DAS.AdminService.Web.Services
{
    public static class TextSanitiser
    {
        public static string SanitiseText(string inputText)
        {
            var text = inputText;

            text = HtmlTagRemover.StripOutTags(text);

            text = ExcelFormulaRemover.StripFormulae(text);

            return text;
        }
    }
}
