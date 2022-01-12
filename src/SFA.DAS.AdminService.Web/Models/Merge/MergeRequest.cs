using System;

namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class MergeRequest
    {
        public Epao PrimaryEpao { get; set; }
        public Epao SecondaryEpao { get; set; }
        public DateTime? SecondaryEpaoEffectiveTo { get; set; }

        public bool Completed { get; private set; }

        public void SetPrimaryEpao(string id, string name)
        {
            PrimaryEpao = new Epao(id, name);
        }

        public void SetSecondaryEpao(string id, string name)
        {
            SecondaryEpao = new Epao(id, name);
        }

        public void SetSecondaryEpaoEffectiveToDate(int day, int month, int year)
        {
            SecondaryEpaoEffectiveTo = new DateTime(year, month, day);
        }

        public void MarkComplete()
        {
            Completed = true;
        }
    }

    public class Epao
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Epao(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
