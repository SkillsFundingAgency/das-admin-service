using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.AdminService.Common.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = ValidationErrorClassName)]
    [HtmlTargetElement("input", Attributes = ValidationErrorClassName)]
    [HtmlTargetElement("fieldset", Attributes = ValidationErrorClassName)]
    public class ValidationClassListedErrorsTagHelper : TagHelper
    {
        public const string ValidationErrorClassName = "sfa-validationerror-class";

        public const string ValidationListAttributeName = "sfa-validation-for-list";

        /// <summary>
        /// This is a CSV list, you can also use it to check for a single entry within the ModelState.
        /// It is not bound to a ModelExpression so will take any value/free-text.
        /// </summary>
        [HtmlAttributeName(ValidationListAttributeName)]
        public string CsvList { get; set; }

        [HtmlAttributeName(ValidationErrorClassName)]
        public string ValidationErrorClass { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(CsvList))
            {
                foreach (var key in CsvList.Split(","))
                {
                    ModelStateEntry entry;
                    ViewContext.ViewData.ModelState.TryGetValue(key, out entry);
                    if (entry == null || !entry.Errors.Any()) continue;

                    var tagBuilder = new TagBuilder(context.TagName);
                    tagBuilder.AddCssClass(ValidationErrorClass);
                    output.MergeAttributes(tagBuilder);
                    break;
                }
            }
        }
    }

}