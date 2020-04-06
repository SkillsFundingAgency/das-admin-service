namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Response { get; set; }
    }
}
