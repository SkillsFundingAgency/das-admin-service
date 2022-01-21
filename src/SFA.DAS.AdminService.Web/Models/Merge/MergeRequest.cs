using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class MergeRequest
    {
        public List<SessionCommand> Actions;

        public Epao PrimaryEpao { get; set; }
        public Epao SecondaryEpao { get; set; }
        public DateTime? SecondaryEpaoEffectiveTo { get; set; }
        public bool Completed { get; set; }

        public SessionCommand PreviousCommand 
        { 
            get => Actions.OrderByDescending(c => c.Order).FirstOrDefault(); 
        }

        public void StartNewRequest()
        {
            PrimaryEpao = null;
            SecondaryEpao = null;
            SecondaryEpaoEffectiveTo = null;
            Completed = false;
            Actions = new List<SessionCommand>();

            PushCommand(new SessionCommand(SessionCommands.StartSession, null, null));
        }

        public void AddSearchEpaoCommand(string type, string searchString)
        {
            if (type == "primary")
            {
                PushCommand(new SessionCommand(SessionCommands.SearchPrimaryEpao, searchString, null));
            }
            else if (type == "secondary")
            {
                PushCommand(new SessionCommand(SessionCommands.SearchSecondaryEpao, searchString, null));
            }
        }

        public void UpdateEpao(string type, string epaoId, string name, long ukprn, string searchString)
        {
            if (type == "primary")
            {
                SetPrimaryEpao(epaoId, name, ukprn, searchString);
            }
            else if (type == "secondary")
            {
                SetSecondaryEpao(epaoId, name, ukprn, searchString);
            }
        }

        public void SetSecondaryEpaoEffectiveToDate(int day, int month, int year)
        {
            SecondaryEpaoEffectiveTo = new DateTime(year, month, day);

            PushCommand(new SessionCommand(SessionCommands.SetSecondaryEpaoEffectiveTo, null, null));
        }

        public void MarkComplete()
        {
            Completed = true;
        }

        public void DeleteLastCommand()
        {
            var lastCommand = Actions.OrderByDescending(c => c.Order).FirstOrDefault();

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

            Actions.Remove(lastCommand);
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
            command.Order = Actions.Count + 1;

            Actions.Add(command);
        }
    }
}
