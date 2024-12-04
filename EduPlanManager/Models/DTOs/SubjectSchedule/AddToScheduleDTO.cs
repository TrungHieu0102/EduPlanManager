namespace EduPlanManager.Models.DTOs.SubjectSchedule
{
    public class AddToScheduleDTO<T> where T : class
    {
       
            public List<Guid> Ids { get; set; }
            public Guid ScheduleId { get; set; }
        
    }
}

