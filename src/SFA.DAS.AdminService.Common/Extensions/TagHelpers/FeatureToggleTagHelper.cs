using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Common.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = FeatureToggleHelperAttributeName)]
    [HtmlTargetElement("a", Attributes = FeatureToggleHelperAttributeName)]
    public class FeatureToggleTagHelper : TagHelper
    {
        private const string FeatureToggleHelperAttributeName = "sfa-feature-toggle";

        private readonly IFeatureToggles _featureToggles;

        [HtmlAttributeName(FeatureToggleHelperAttributeName)]
        public string FeatureToggle { get; set; }

        public FeatureToggleTagHelper(IFeatureToggles featureToggles)
        {
            _featureToggles = featureToggles;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsFeatureToggleEnabled(FeatureToggle))
            {
                output.SuppressOutput();
            }
        }

        public bool IsFeatureToggleEnabled(string featureToggle)
        {
            var toggleEnabled = false;

            if (!string.IsNullOrWhiteSpace(featureToggle))
            {
                try
                {
                    var property = _featureToggles.GetType().GetProperty(featureToggle);
                    var propertyValue = property.GetValue(_featureToggles, null);
                    toggleEnabled = Convert.ToBoolean(propertyValue);
                }
                catch (SystemException ex) when (ex is InvalidCastException || ex is FormatException)
                {
                    toggleEnabled = false;
                    Debug.WriteLine($"FeatureToogle '{featureToggle}' is not in the expected format");
                }
                catch (SystemException ex) when (ex is NullReferenceException)
                {
                    toggleEnabled = false;
                    Debug.WriteLine($"FeatureToogle '{featureToggle}' is not defined in the configuration");
                }
            }

            return toggleEnabled;
        }
    }
}
