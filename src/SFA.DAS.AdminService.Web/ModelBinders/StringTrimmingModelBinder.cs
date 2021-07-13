using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.AdminService.Web.ModelBinders
{
    public class StringTrimmingModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.ModelType == typeof(string))
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult != ValueProviderResult.None && valueProviderResult.FirstValue is string input)
                {
                    bindingContext.Result = ModelBindingResult.Success(input?.Trim());
                }
            }

            return Task.CompletedTask;
        }
    }
}
