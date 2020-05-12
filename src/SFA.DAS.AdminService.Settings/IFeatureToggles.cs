namespace SFA.DAS.AdminService.Settings
{
    public interface IFeatureToggles
    {
        bool EnableRoatpAssessorReview { get; set; }
        bool EnableRoatpFinancialReview { get; set; }
        bool EnableRoatpGatewayReview { get; set; }
        bool EnableRoatpSnapshot { get; set; }
        bool EnableRoatpOversightReview { get; set; }
    }
}