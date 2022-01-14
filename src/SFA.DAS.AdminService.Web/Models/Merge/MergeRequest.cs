using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class MergeRequest
    {
        public Epao PrimaryEpao { get; set; }
        public Epao SecondaryEpao { get; set; }
        public DateTime? SecondaryEpaoEffectiveTo { get; set; }
        public List<SessionCommand> Actions { get; set; } = new List<SessionCommand>();

        public bool Completed { get; set; }

        public void SetPrimaryEpao(string id, string name)
        {
            PrimaryEpao = new Epao(id, name);

            PushCommand(new SessionCommand(SessionCommands.ConfirmPrimaryEpao, null, id));
        }

        public void SetSecondaryEpao(string id, string name)
        {
            SecondaryEpao = new Epao(id, name);

            PushCommand(new SessionCommand(SessionCommands.ConfirmSecondaryEpao, null, id));
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

        public SessionCommand Peek() => Actions.OrderByDescending(c => c.Order).FirstOrDefault();

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

        public void PushCommand(SessionCommand command)
        {
            command.Order = Actions.Count + 1;

            Actions.Add(command);
        }
    }

    public class Epao
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ukprn { get; set; }

        public Epao(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
