namespace EduPlanManager.Models.DTOs.Respone
{
    public class ResultPage<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; } = string.Empty;
        public IEnumerable<T>? Data { get; set; }
        public int? TotalCount { get; set; } 
        public int ?TotalPages { get; set; } 
        public int ?CurrentPage { get; set; } 
    }
}
