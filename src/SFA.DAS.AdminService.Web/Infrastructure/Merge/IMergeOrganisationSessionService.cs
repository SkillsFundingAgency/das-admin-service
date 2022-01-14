using SFA.DAS.AdminService.Web.Models.Merge;

namespace SFA.DAS.AdminService.Web.Infrastructure.Merge
{
    public interface IMergeOrganisationSessionService
    {
        MergeRequest GetMergeRequest();
        void StartNewMergeRequest();

        Epao GetPrimaryEpao();

        Epao GetSecondaryEpao();

        void UpdateEpao(string type, string id, string name);
        void SetSecondaryEpaoEffectiveToDate(int day, int month, int year);
        void MarkComplete();
        void DeleteLastCommand();
        void AddSearchEpaoCommand(string type, string searchString);
    }
}
