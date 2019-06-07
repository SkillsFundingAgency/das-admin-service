namespace SFA.DAS.AdminService.Application.Interfaces
{
    public interface IEpaOrganisationSearchValidator
    {
        bool IsValidEpaOrganisationId(string organisationIdToCheck);
        bool IsValidUkprn(string stringToCheck);
    }
}
