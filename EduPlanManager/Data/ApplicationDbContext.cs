using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
    {
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentSchedule> StudentSchedules { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AcademicTerm> AcademicTerms { get; set; }
        public DbSet<EnrollmentRequest> EnrollmentRequests { get; set; }
        public DbSet<SubjectCategory> SubjectCategories { get; set; }
        public DbSet<Class> Classes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho bảng Grade
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

            // Cấu hình cho bảng StudentSchedule
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

            // Cấu hình cho bảng Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AcademicTerm>()
            .HasIndex(a => new { a.Year, a.Semester, a.StartDate, a.EndDate })
            .IsUnique();
            // Xác định mối quan hệ nhiều-nhiều giữa User và Class
            modelBuilder.Entity<User>()
                .HasMany(u => u.Classes)
                .WithMany(c => c.Users)
                .UsingEntity(j => j.ToTable("UserClasses"));  // Tên bảng trung gian, EF tự tạo bảng này

            // Xác định mối quan hệ nhiều-nhiều giữa Class và Subject
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Subjects)
                .WithMany(s => s.Classes)
                .UsingEntity(j => j.ToTable("ClassSubjects"));  // Tên bảng trung gian, EF tự tạo bảng này
        }


    }
}
