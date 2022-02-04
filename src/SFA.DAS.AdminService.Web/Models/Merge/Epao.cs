namespace SFA.DAS.AdminService.Web.Models.Merge
{
    public class Epao
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Ukprn { get; set; }

        public Epao(string id, string name, long? ukprn)
        {
            Id = id;
            Name = name;
            Ukprn = ukprn;
        }
    }
}
