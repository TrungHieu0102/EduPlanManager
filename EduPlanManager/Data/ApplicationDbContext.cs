using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentSchedule> StudentSchedules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AcademicTerm> AcademicTerms { get; set; }
        public DbSet<SubjectCategory> SubjectCategories { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<SubjectSchedule> SubjectSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ràng buộc cho Grade
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ràng buộc cho StudentSchedule
            modelBuilder.Entity<StudentSchedule>()
                .HasOne(ss => ss.Student)
                .WithMany(u => u.Schedules)
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentSchedule>()
                .HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSchedules)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ràng buộc cho Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.SubjectSchedule)
                .WithMany(ss => ss.Enrollments)
                .HasForeignKey(e => e.SubjectScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ràng buộc duy nhất cho AcademicTerm
            modelBuilder.Entity<AcademicTerm>()
                .HasIndex(a => new { a.Year, a.Semester, a.StartDate, a.EndDate })
                .IsUnique();

            // Thiết lập quan hệ nhiều-nhiều
            modelBuilder.Entity<User>()
                .HasMany(u => u.Classes)
                .WithMany(c => c.Users)
                .UsingEntity(j => j.ToTable("UserClasses"));

            modelBuilder.Entity<Class>()
                .HasMany(c => c.Subjects)
                .WithMany(s => s.Classes)
                .UsingEntity(j => j.ToTable("ClassSubjects"));

            // Ràng buộc cho Subject và SubjectSchedule
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasMany(s => s.Schedules)
                      .WithMany(sch => sch.Subjects)
                      .UsingEntity<Dictionary<string, object>>(
                          "SubjectSubjectSchedule",
                          join => join
                              .HasOne<SubjectSchedule>()
                              .WithMany()
                              .HasForeignKey("SubjectScheduleId")
                              .OnDelete(DeleteBehavior.Restrict), // Không cascade delete
                          join => join
                              .HasOne<Subject>()
                              .WithMany()
                              .HasForeignKey("SubjectId")
                              .OnDelete(DeleteBehavior.Restrict), // Không cascade delete
                          join =>
                          {
                              join.HasKey("SubjectId", "SubjectScheduleId");
                          });
            });

            // Ràng buộc cho SubjectSchedule
            modelBuilder.Entity<SubjectSchedule>(entity =>
            {
                entity.HasKey(ss => ss.Id);

                entity.Property(ss => ss.DayOfWeek)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(ss => ss.Session)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(ss => ss.StartTime)
                      .IsRequired();

                entity.Property(ss => ss.EndTime)
                      .IsRequired();

                entity.Property(ss => ss.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(ss => ss.UpdatedAt)
                      .IsRequired(false);
            });
        }
    }
}
