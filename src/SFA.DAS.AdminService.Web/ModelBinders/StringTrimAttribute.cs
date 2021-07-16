using System;

namespace SFA.DAS.AdminService.Web.ModelBinders
{
    /// <summary>
    /// Denotes a data field, class property or method parameter that should be trimmed of whitespace during binding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Support for trimming is implmented in the model binder, as currently
    /// Data Annotations provides no mechanism to coerce the value.
    /// </para>
    /// <para>
    /// This attribute does not imply that empty strings should be converted to null.
    /// When that is required you must additionally use the <see cref="System.ComponentModel.DataAnnotations.DisplayFormatAttribute.ConvertEmptyStringToNull"/>
    /// option to control what happens to empty strings.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StringTrimAttribute : Attribute
    {
    }
}
