using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Extensions
{
    public class RoatpAssessorApplicationData
    {
        private const string Key_Apply_ProviderRoute = "Apply-ProviderRoute";
        private const string Key_UKRLP_LegalName = "UKRLP-LegalName";
        private const string Key_UKPRN = "UKPRN";

        private readonly Dictionary<string, object> _applicationData;

        public RoatpAssessorApplicationData(Dictionary<string, object> applicationData)
        {
            _applicationData = applicationData;
        }

        public string UKRLP_LegalName => _applicationData[Key_UKRLP_LegalName].ToString();

        public ProviderRoutes Apply_ProviderRoute => Enum.Parse<ProviderRoutes>(_applicationData[Key_Apply_ProviderRoute].ToString());
        
        public string UKPRN => _applicationData[Key_UKPRN].ToString();
    }
}
