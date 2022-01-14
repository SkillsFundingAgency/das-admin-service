﻿using SFA.DAS.AdminService.Web.Models.Merge;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Infrastructure.Merge
{
    public class MergeOrganisationSessionService : IMergeOrganisationSessionService
    {
        private ISessionService _sessionService;

        private const string _mergeOrganisationsSessionKey = "Merge_Organisations";

        public MergeOrganisationSessionService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void StartNewMergeRequest()
        {
            _sessionService.Remove(_mergeOrganisationsSessionKey);

            var mergeRequest = new MergeRequest();

            mergeRequest.PushCommand(new SessionCommand(SessionCommands.StartSession, null, null));

            _sessionService.Set(_mergeOrganisationsSessionKey, mergeRequest);
        }

        public MergeRequest GetMergeRequest()
        {
            return _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);
        }

        public Epao GetPrimaryEpao()
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            return request.PrimaryEpao;
        }

        public Epao GetSecondaryEpao()
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            return request.SecondaryEpao;
        }

        public void UpdateEpao(string type, string id, string name)
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            if (type == "primary")
            {
                request.SetPrimaryEpao(id, name);
            }
            else if (type == "secondary")
            {
                request.SetSecondaryEpao(id, name);
            }

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }

        public void SetSecondaryEpaoEffectiveToDate(int day, int month, int year)
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            request.SetSecondaryEpaoEffectiveToDate(day, month, year);

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }

        public void MarkComplete()
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            request.MarkComplete();

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }

        public void DeleteLastCommand()
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            request.DeleteLastCommand();

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }

        public void AddSearchEpaoCommand(string type, string searchString)
        {
            var request = _sessionService.Get<MergeRequest>(_mergeOrganisationsSessionKey);

            request.AddSearchEpaoCommand(type, searchString);

            _sessionService.Set(_mergeOrganisationsSessionKey, request);
        }
    }
}
