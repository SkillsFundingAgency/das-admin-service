using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.AdminService.Web.Attributes;

namespace SFA.DAS.AdminService.Web.ModelBinders
{
    public class SuppressBindingErrorsModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var property = context.Metadata.ContainerType?.GetProperty(context.Metadata.PropertyName);
            if (property != null && property.IsDefined(typeof(SuppressBindingErrorsAttribute), true))
            {
                return new BinderTypeModelBinder(typeof(SuppressBindingErrorsModelBinder));
            }

            return null;
        }
    }
}
