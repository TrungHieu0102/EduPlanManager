using EduPlanManager.Data;
using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.Entities;
using EduPlanManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Repositories
{
    public class AcademicTermRepository(ApplicationDbContext context) : RepositoryBase<AcademicTerm, Guid>(context), IAcademicTermRepository
    {
        public async Task<AcademicTerm?> CheckExists(CreateUpdateAcademicTermDTO createAcademicTerm)
        {
            var existingTerm = await _context.AcademicTerms
             .FirstOrDefaultAsync(t => t.Year == createAcademicTerm.Year &&
                                       t.Semester == createAcademicTerm.Semester &&
                                       t.StartDate == createAcademicTerm.StartDate &&
                                       t.EndDate == createAcademicTerm.EndDate);
            return existingTerm;
        }
    }
}
