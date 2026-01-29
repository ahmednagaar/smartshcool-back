using Nafes.API.Modules;

namespace Nafes.API.Services;

public interface IJwtService
{
    string GenerateAccessToken(Admin admin);
    string GenerateStudentToken(Student student);
    string GenerateParentToken(Parent parent);
    string GenerateRefreshToken();
    long? ValidateToken(string token);
}
