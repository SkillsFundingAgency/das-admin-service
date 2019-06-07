using System;
using System.Drawing;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public interface IRegisterValidator
    {
        ValidationResponse CheckDateIsEmptyOrValid(string day, string month, string year, 
            string dayFieldName, string monthFieldName, string yearFieldName, string dateFieldName, string dateFieldDescription);

        

    }
}