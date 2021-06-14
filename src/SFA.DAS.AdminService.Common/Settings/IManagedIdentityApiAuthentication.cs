namespace SFA.DAS.AdminService.Common.Settings
{
    public interface IManagedIdentityApiAuthentication
    {
        string Identifier { get; set; }
        string ApiBaseAddress { get; set; }
    }
}