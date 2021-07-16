using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace SFA.DAS.AdminService.Web.ModelBinders
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class StringTrimmingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var metadata = context.Metadata as DefaultModelMetadata;

            if (!metadata.IsComplexType && metadata.ModelType == typeof(string)
                && metadata.Attributes.Attributes.OfType<StringTrimAttribute>().Any()) // Note: Must opt-in
            {
                return new BinderTypeModelBinder(typeof(StringTrimmingModelBinder));
            }

            return null;
        }
    }
}
