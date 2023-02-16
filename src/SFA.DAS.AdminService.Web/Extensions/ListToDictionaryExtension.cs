using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class ListToDictionaryExtension
    {
        public static Dictionary<string, object> ToDictionary(
            this CertificateSummaryResponse item, string toExcelMappings, string[] datesToFormatToShortString)
        {
            return ToDictionary<object>(item, toExcelMappings, datesToFormatToShortString);
        }

        private static Dictionary<string, object> ToDictionary<TValue>(object obj, string excelAttributeMapping, string[] datesToFormatToShortString)
        {
            var json = JsonSerializer.Serialize(obj);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, TValue>>(json);
            var destDictionary = new Dictionary<string, object>();
            MatchAndCreateNewMapping(excelAttributeMapping, dictionary, destDictionary, datesToFormatToShortString);
            return destDictionary;
        }

        private static void MatchAndCreateNewMapping(string mappingJson, dynamic sourceJson, IDictionary<string, object> destinationDictionary, string[] datesToFormatToShortString)
        {
            var mappings = JsonSerializer.Deserialize<Dictionary<string, object>>(mappingJson);
            foreach (var mapping in mappings)
            {
                foreach (KeyValuePair<string, object> source in sourceJson)
                {
                    if (mapping.Key != source.Key) continue;
                    destinationDictionary.Add(mapping.Value.ToString(),
                        datesToFormatToShortString.Contains(mapping.Key)
                            ? ((DateTime?) source.Value)?.ToShortDateString()
                            : source.Value);
                    break;
                }
            }
        }
    }
}
