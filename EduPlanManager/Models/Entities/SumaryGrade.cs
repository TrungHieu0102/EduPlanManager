using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlanManager.Models.Entities
{
    [Table("SumaryGrade")]

    public class SumaryGrade
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid AcademicTermId { get; set; }
        public Status Status { get; set; }
        public float Summary { get; set; }
        public bool NeedsImprovement { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
    public enum Status
    {
        Pass = 1,
        Fail = 2
    }

}
