﻿using SFA.DAS.AdminService.Web.Models.Merge;

namespace SFA.DAS.AdminService.Web.Infrastructure.Merge
{
    public interface IMergeOrganisationSessionService
    {
        void StartNewMergeRequest();
        MergeRequest GetMergeRequest();
        void UpdateMergeRequest(MergeRequest mergeRequest);


        void MarkComplete();
        void DeleteLastCommand();
        void AddSearchEpaoCommand(string type, string searchString);
    }
}
