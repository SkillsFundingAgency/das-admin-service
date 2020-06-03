using System;
using System.Drawing;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public interface IRegisterValidator
    {
        ValidationResponse CheckDateIsEmptyOrValid(string day, string month, string year, 
            string dayFieldName, string monthFieldName, string yearFieldName, string dateFieldName, string dateFieldDescription);

        

    }
}