using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Helpers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Validators
{
    //public class BaseValidator<T> : AbstractValidator<T>
    //{
    //    private IRegisterValidator _registerValidator;

    //    public BaseValidator(IRegisterValidator registerValidator)
    //    {
    //        _registerValidator = registerValidator;
    //    }

    //    protected static void CreateFailuresInContext(IEnumerable<ValidationErrorDetail> errs, CustomContext context)
    //    {
    //        foreach (var error in errs)
    //        {
    //            context.AddFailure(error.Field, error.ErrorMessage);
    //        }
    //    }

    //    protected static DateTime? ConstructDate(string dayString, string monthString, string yearString)
    //    {

    //        if (!int.TryParse(dayString, out var day) || !int.TryParse(monthString, out var month) ||
    //            !int.TryParse(yearString, out var year)) return null;

    //        if (!IsValidDate(year, month, day))
    //            return null;

    //        return new DateTime(year, month, day);
    //    }

    //    protected static bool IsValidDate(int year, int month, int day)
    //    {
    //        if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
    //            return false;

    //        if (month < 1 || month > 12)
    //            return false;

    //        return day > 0 && day <= DateTime.DaysInMonth(year, month);
    //    }

    //    protected ValidationResponse CheckEffectiveFromDateIsEmptyOrValid(string year, string month, string day)
    //    {
    //        return _registerValidator.CheckDateIsEmptyOrValid(day, month, year, "EffectiveFromDay", "EffectiveFromMonth", "EffectiveFromYear", "EffectiveFrom", "Effective From");
    //    }
    //}
}
