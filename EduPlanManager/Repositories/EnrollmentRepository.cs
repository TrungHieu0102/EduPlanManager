﻿using EduPlanManager.Data;
using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.SubjectSchedule;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class EnrollmentRepository(ApplicationDbContext context) : RepositoryBase<Enrollment, Guid>(context), IEnrollmentRepository
    {
        public async Task AddEnrollment(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }
        public async Task<List<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId)
        {
            var studentClasses = await _context.Classes
                .Where(c => c.Users.Any(u => u.Id == studentId))
                .Select(c => c.Id)
                .ToListAsync();

            var subjectSchedules = await _context.SubjectSchedules
                .Where(ss => ss.Subjects.Any(s => s.Classes.Any(c => studentClasses.Contains(c.Id))))
                .SelectMany(ss => ss.Subjects.Select(subject => new EnrollmentListRespone
                {
                    ScheduleId = ss.Id,
                    SubjectId = subject.Id,
                    SubjectCode = subject.Code,
                    SubjectName = subject.Name,
                    DayOfWeek = ss.DayOfWeek,
                    Session = ss.Session,
                    StartTime = ss.StartTime,
                    EndTime = ss.EndTime
                }))
                .ToListAsync();
            return subjectSchedules;
        }

        public async Task<bool> HasConflictSchedule(Guid studentId, Guid subjectScheduleId)
        {
            return await _context.Enrollments
              .AnyAsync(e =>
              e.StudentId == studentId &&
              e.SubjectScheduleId == subjectScheduleId &&
              (e.Status == EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Approved));
        }

        public async Task<bool> HasDuplicateSubject(Guid studentId, Guid subjectId, Guid academicTermId)
        {
            return await _context.Enrollments
           .AnyAsync(e =>
               e.StudentId == studentId &&
               e.SubjectId == subjectId &&
               e.AcademicTermId == academicTermId &&
               (e.Status == EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Approved));
        }

        public async Task<bool> IsStudentInClass(Guid studentId, Guid subjectId)
        {
            return await _context.Classes
                       .Where(c => c.Users.Any(u => u.Id == studentId))
                       .AnyAsync(c => c.Subjects.Any(s => s.Id == subjectId));
        }


    }
}