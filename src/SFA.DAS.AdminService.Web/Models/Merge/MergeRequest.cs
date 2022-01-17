using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class MergeRequest
    {
        private List<SessionCommand> _actions;

        public Epao PrimaryEpao { get; set; }
        public Epao SecondaryEpao { get; set; }
        public DateTime? SecondaryEpaoEffectiveTo { get; set; }
        public bool Completed { get; set; }
        public SessionCommand PreviousCommand 
        { 
            get => _actions.OrderByDescending(c => c.Order).FirstOrDefault(); 
        }

        public MergeRequest()
        {
            _actions = new List<SessionCommand>();

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

        public void UpdateEpao(string type, string epaoId, string name, long ukprn)
        {
            if (type == "primary")
            {
                SetPrimaryEpao(epaoId, name, ukprn);
            }
            else if (type == "secondary")
            {
                SetSecondaryEpao(epaoId, name, ukprn);
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
            var lastCommand = _actions.OrderByDescending(c => c.Order).FirstOrDefault();

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

            _actions.Remove(lastCommand);
        } 

        private void SetPrimaryEpao(string id, string name, long ukprn)
        {
            PrimaryEpao = new Epao(id, name, ukprn);

            PushCommand(new SessionCommand(SessionCommands.ConfirmPrimaryEpao, null, id));
        }

        private void SetSecondaryEpao(string id, string name, long ukprn)
        {
            SecondaryEpao = new Epao(id, name, ukprn);

            PushCommand(new SessionCommand(SessionCommands.ConfirmSecondaryEpao, null, id));
        }

        private void PushCommand(SessionCommand command)
        {
            command.Order = _actions.Count + 1;

            _actions.Add(command);
        }
    }

    public class Epao
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Ukprn { get; set; }

        public Epao(string id, string name, long? ukprn )
        {
            Id = id;
            Name = name;
            Ukprn = ukprn;
        }
    }
}
