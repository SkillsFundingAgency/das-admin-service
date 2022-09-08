﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace SFA.DAS.AdminService.Web.Services
{
    public class CsvExportService : ICsvExportService
    {
        public byte[] WriteCsvToByteArray<T, TU>(IEnumerable<T> records) where TU : ClassMap<T>
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                SanitizeForInjection = true,
            };
            
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, csvConfiguration))
            {
                csvWriter.Context.RegisterClassMap<TU>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
