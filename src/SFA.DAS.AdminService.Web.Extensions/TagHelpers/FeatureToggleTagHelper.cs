using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Settings;

namespace SFA.DAS.AdminService.Web.Extensions.TagHelpers
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

        public FeatureToggleTagHelper(ILogger<FeatureToggleTagHelper> logger, IWebConfiguration configuration)
        {
            _logger = logger;
            _featureToggles = configuration?.FeatureToggles;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsFeatureToggleEnabled(_logger, _featureToggles, FeatureToggle))
            {
                output.SuppressOutput();
            }
        }

        private static bool IsFeatureToggleEnabled(ILogger<FeatureToggleTagHelper> _logger, IFeatureToggles _featureToggles, string featureToggle)
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
                    if (_logger != null)
                    {
                        _logger.LogError(ex, $"FeatureToogle '{featureToggle}' is not in the expected format");
                    }
                }
                catch (SystemException ex) when (ex is NullReferenceException)
                {
                    toggleEnabled = false;
                    if (_logger != null)
                    {
                        _logger.LogError(ex, $"FeatureToogle '{featureToggle}' is not defined in the configuration");
                    }
                }
            }

            return toggleEnabled;
        }
    }
}
