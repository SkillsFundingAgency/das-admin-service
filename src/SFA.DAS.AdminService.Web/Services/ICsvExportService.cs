using System.Collections.Generic;
using CsvHelper.Configuration;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface ICsvExportService
    {
        byte[] WriteCsvToByteArray<T, TU>(IEnumerable<T> records) where TU : ClassMap<T>;
    }
}
