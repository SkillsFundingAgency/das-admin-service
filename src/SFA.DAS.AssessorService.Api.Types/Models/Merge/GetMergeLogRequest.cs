namespace SFA.DAS.AssessorService.Api.Types.Models.Merge
{
    public class GetMergeLogRequest
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string PrimaryEPAOId { get; set; }
        public string SecondaryEPAOId { get; set; }
    }
}
