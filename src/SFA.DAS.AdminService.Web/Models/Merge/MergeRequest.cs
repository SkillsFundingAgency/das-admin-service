using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class MergeRequest
    {
        public List<SessionCommand> Commands { get; set; }

        public Epao PrimaryEpao { get; set; }
        public Epao SecondaryEpao { get; set; }
        public DateTime? SecondaryEpaoEffectiveTo { get; set; }
        public bool Completed { get; set; }

        public SessionCommand PreviousCommand 
        { 
            get => Commands.OrderByDescending(c => c.Order).FirstOrDefault(); 
        }

        public void StartNewRequest()
        {
            PrimaryEpao = null;
            SecondaryEpao = null;
            SecondaryEpaoEffectiveTo = null;
            Completed = false;
            Commands = new List<SessionCommand>();

            PushCommand(new SessionCommand(SessionCommands.StartSession, null, null));
        }

        public void AddSearchEpaoCommand(string mergeOrganisationType, string searchString)
        {
            if (mergeOrganisationType == MergeOrganisationType.Primary)
            {
                PushCommand(new SessionCommand(SessionCommands.SearchPrimaryEpao, searchString, null));
            }
            else if (mergeOrganisationType == MergeOrganisationType.Secondary)
            {
                PushCommand(new SessionCommand(SessionCommands.SearchSecondaryEpao, searchString, null));
            }
        }

        public void UpdateEpao(string mergeOrganisationType, string epaoId, string name, long ukprn, string searchString)
        {
            if (mergeOrganisationType == MergeOrganisationType.Primary)
            {
                SetPrimaryEpao(epaoId, name, ukprn, searchString);
            }
            else if (mergeOrganisationType == MergeOrganisationType.Secondary)
            {
                SetSecondaryEpao(epaoId, name, ukprn, searchString);
            }
        }

        public bool SetSecondaryEpaoEffectiveToDate(string dayString, string monthString, string yearString)
        {
            try
            {
                var day = int.Parse(dayString);
                var month = int.Parse(monthString);
                var year = int.Parse(yearString);

                var dateTime = new DateTime(year, month, day);

                SecondaryEpaoEffectiveTo = dateTime;

                PushCommand(new SessionCommand(SessionCommands.SetSecondaryEpaoEffectiveTo, null, null));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void MarkComplete()
        {
            Completed = true;
        }

        public void DeleteLastCommand()
        {
            var lastCommand = Commands.OrderByDescending(c => c.Order).FirstOrDefault();

            switch (lastCommand.CommandName) 
            {
                case SessionCommands.ConfirmPrimaryEpao:
                    PrimaryEpao = null;
                    break;

                case SessionCommands.ConfirmSecondaryEpao:
                    SecondaryEpao = null;
                    break;

                case SessionCommands.SetSecondaryEpaoEffectiveTo:
                    SecondaryEpaoEffectiveTo = null;
                    break;
            }

            Commands.Remove(lastCommand);
        } 

        private void SetPrimaryEpao(string id, string name, long ukprn, string searchString)
        {
            PrimaryEpao = new Epao(id, name, ukprn);

            PushCommand(new SessionCommand(SessionCommands.ConfirmPrimaryEpao, searchString, id));
        }

        private void SetSecondaryEpao(string id, string name, long ukprn, string searchString)
        {
            SecondaryEpao = new Epao(id, name, ukprn);

            PushCommand(new SessionCommand(SessionCommands.ConfirmSecondaryEpao, searchString, id));
        }

        private void PushCommand(SessionCommand command)
        {
            command.Order = Commands.Count + 1;

            Commands.Add(command);
        }
    }
}
