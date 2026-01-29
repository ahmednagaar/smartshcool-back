using Microsoft.EntityFrameworkCore;
using Nafes.API.Data;
using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Student>> GetByGradeAsync(string grade)
    {
        return await _dbSet
            .Where(s => s.Grade == grade && !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<Student?> GetByStudentCodeAsync(string studentCode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.StudentCode == studentCode && !s.IsDeleted && s.IsActive);
    }
}
