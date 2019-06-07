namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface ISpecialCharacterCleanserService
    {
        string CleanseStringForSpecialCharacters(string inputString);
        string UnescapeAndRemoveNonAlphanumericCharacters(string inputString);
    }
}