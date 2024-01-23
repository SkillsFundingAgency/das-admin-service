namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    public class UkrlpProviderDetails
    {
        public bool IsMatched => !string.IsNullOrEmpty(LegalName);
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
    }
}
