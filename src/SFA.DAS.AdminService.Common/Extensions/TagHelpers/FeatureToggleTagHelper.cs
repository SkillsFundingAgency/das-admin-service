using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.AdminService.Common.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = FeatureToggleHelperAttributeName)]
    [HtmlTargetElement("a", Attributes = FeatureToggleHelperAttributeName)]
    public class FeatureToggleTagHelper : TagHelper
    {
        private const string FeatureToggleHelperAttributeName = "sfa-feature-toggle";

        private readonly ILogger<FeatureToggleTagHelper> _logger;
        private readonly IFeatureToggles _featureToggles;

        [HtmlAttributeName(FeatureToggleHelperAttributeName)]
        public string FeatureToggle { get; set; }

        public FeatureToggleTagHelper(ILogger<FeatureToggleTagHelper> logger, IFeatureToggles featureToggles)
        {
            _logger = logger;
            _featureToggles = featureToggles;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsFeatureToggleEnabled())
            {
                output.SuppressOutput();
            }
        }

        private bool IsFeatureToggleEnabled()
        {
            var toggleEnabled = false;

            if (!string.IsNullOrWhiteSpace(FeatureToggle))
            {
                try
                {
                    var property = _featureToggles.GetType().GetProperty(FeatureToggle);
                    var propertyValue = property.GetValue(_featureToggles, null);
                    toggleEnabled = Convert.ToBoolean(propertyValue);
                }
                catch (SystemException ex) when (ex is InvalidCastException || ex is FormatException)
                {
                    toggleEnabled = false;
                    _logger.LogError(ex, $"FeatureToogle '{FeatureToggle}' is not in the expected format");
                }
                catch (SystemException ex) when (ex is NullReferenceException)
                {
                    toggleEnabled = false;
                    _logger.LogError(ex, $"FeatureToogle '{FeatureToggle}' is not defined in the configuration");
                }
            }

            return toggleEnabled;
        }
    }
}
