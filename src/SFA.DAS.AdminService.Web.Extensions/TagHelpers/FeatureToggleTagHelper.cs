using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.AdminService.Settings;

namespace SFA.DAS.AdminService.Web.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = FeatureToggleHelperAttributeName)]
    [HtmlTargetElement("a", Attributes = FeatureToggleHelperAttributeName)]
    public class FeatureToggleTagHelper : TagHelper
    {
        private const string FeatureToggleHelperAttributeName = "sfa-feature-toggle";

        private readonly IFeatureToggles _featureToggles;

        [HtmlAttributeName(FeatureToggleHelperAttributeName)]
        public string FeatureToggle { get; set; }

        public FeatureToggleTagHelper(IWebConfiguration configuration)
        {
            _featureToggles = configuration?.FeatureToggles;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsFeatureToggleEnabled(_featureToggles, FeatureToggle))
            {
                output.SuppressOutput();
            }
        }

        private static bool IsFeatureToggleEnabled(IFeatureToggles _featureToggles, string featureToggle)
        {
            var toggleEnabled = false;

            if (_featureToggles != null && !string.IsNullOrWhiteSpace(featureToggle))
            {
                try
                {
                    var property = _featureToggles.GetType().GetProperty(featureToggle);
                    var propertyValue = property.GetValue(_featureToggles, null);

                    if (propertyValue != null)
                    {
                        toggleEnabled = Convert.ToBoolean(propertyValue);
                    }
                }
                catch (SystemException ex) when (ex is InvalidCastException || ex is FormatException)
                {
                    toggleEnabled = false;
                }
            }

            return toggleEnabled;
        }
    }
}
