using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace SFA.DAS.AdminService.Web.Models.Roatp
{
    public class BooleanConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var boolValue = (bool)value;

            return boolValue ? "Y" : "N";
        }
    }
}
