namespace EduPlanManager.Models.DTOs.AcademicTerm
{
    public class AcademicTermDTO
    {
        public int Year { get; set; } 
        public int Semester { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
