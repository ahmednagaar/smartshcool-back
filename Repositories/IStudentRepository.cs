using Nafes.API.Modules;

namespace Nafes.API.Repositories;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<IEnumerable<Student>> GetByGradeAsync(string grade);
    Task<Student?> GetByStudentCodeAsync(string studentCode);
}
