using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Nafes.API.Data;
using Nafes.API.DTOs.Student;
using Nafes.API.Modules;
using Nafes.API.Services;

namespace Nafes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public StudentController(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<StudentLoginResponseDto>> Register([FromBody] StudentRegisterDto registerDto)
    {
        // Validate PIN (must be 4 digits)
        if (string.IsNullOrEmpty(registerDto.Pin) || registerDto.Pin.Length != 4 || !registerDto.Pin.All(char.IsDigit))
        {
            return BadRequest(new { message = "رمز PIN يجب أن يكون 4 أرقام" }); // PIN must be 4 digits
        }

        // Generate unique student code
        var studentCode = await GenerateUniqueStudentCode();

        var student = new Student
        {
            Name = registerDto.Name,
            Age = registerDto.Age,
            Grade = registerDto.Grade,
            StudentCode = studentCode,
            PinHash = _passwordHasher.HashPassword(registerDto.Pin),
            IsActive = true
        };

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.CommitAsync();

        var token = _jwtService.GenerateStudentToken(student);

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, new StudentLoginResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            StudentCode = student.StudentCode,
            Grade = student.Grade,
            AccessToken = token
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<StudentLoginResponseDto>> Login([FromBody] StudentLoginDto loginDto)
    {
        var student = await _unitOfWork.Students.GetByStudentCodeAsync(loginDto.StudentCode);

        if (student == null)
        {
            return Unauthorized(new { message = "رمز الطالب غير صحيح" }); // Invalid student code
        }

        if (string.IsNullOrEmpty(student.PinHash))
        {
            return Unauthorized(new { message = "الحساب غير مفعل. يرجى التسجيل أولاً" }); // Account not activated
        }

        if (!_passwordHasher.VerifyPassword(loginDto.Pin, student.PinHash))
        {
            return Unauthorized(new { message = "رمز PIN غير صحيح" }); // Invalid PIN
        }

        var token = _jwtService.GenerateStudentToken(student);

        return Ok(new StudentLoginResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            StudentCode = student.StudentCode,
            Grade = student.Grade,
            AccessToken = token
        });
    }

    private async Task<string> GenerateUniqueStudentCode()
    {
        var random = new Random();
        string code;
        bool exists;

        do
        {
            var number = random.Next(1000, 9999);
            code = $"NAF-{number}";
            var existingStudent = await _unitOfWork.Students.GetByStudentCodeAsync(code);
            exists = existingStudent != null;
        } while (exists);

        return code;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentGetDto>>> GetAll()
    {
        var students = await _unitOfWork.Students.GetAllAsync();
        var studentDtos = _mapper.Map<IEnumerable<StudentGetDto>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentGetDto>> GetById(long id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
            return NotFound(new { message = "الطالب غير موجود" }); // Student not found

        var studentDto = _mapper.Map<StudentGetDto>(student);
        return Ok(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentGetDto>> Create([FromBody] StudentCreateDto createDto)
    {
        var student = _mapper.Map<Student>(createDto);
        student.StudentCode = await GenerateUniqueStudentCode();
        
        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.CommitAsync();

        var studentDto = _mapper.Map<StudentGetDto>(student);
        
        // Audit Log
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync("Create", "Student", student.Id.ToString(), $"Created student: {student.Name} ({student.StudentCode})", adminId, adminName);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, studentDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudentGetDto>> Update(long id, [FromBody] StudentUpdateDto updateDto)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
            return NotFound(new { message = "الطالب غير موجود" });

        _mapper.Map(updateDto, student);
        _unitOfWork.Students.Update(student);
        
        // Audit Log
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync("Update", "Student", student.Id.ToString(), $"Updated student: {student.Name}", adminId, adminName);
        
        await _unitOfWork.CommitAsync();

        var studentDto = _mapper.Map<StudentGetDto>(student);
        return Ok(studentDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
            return NotFound(new { message = "الطالب غير موجود" });

        _unitOfWork.Students.Remove(student);
        
        // Audit Log
        var adminId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
        await _unitOfWork.AuditLogs.LogAsync("Delete", "Student", student.Id.ToString(), $"Deleted student: {student.Name}", adminId, adminName);

        await _unitOfWork.CommitAsync();

        return Ok(new { message = "تم حذف الطالب بنجاح" }); // Student deleted successfully
    }

    [HttpGet("grade/{grade}")]
    public async Task<ActionResult<IEnumerable<StudentGetDto>>> GetByGrade(string grade)
    {
        var students = await _unitOfWork.Students.GetByGradeAsync(grade);
        var studentDtos = _mapper.Map<IEnumerable<StudentGetDto>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportStudents()
    {
        var students = await _unitOfWork.Students.GetAllAsync();
        var builder = new StringBuilder();
        
        // Add UTF-8 BOM for Excel compatibility
        builder.Append('\uFEFF');
        
        // Header
        builder.AppendLine("المعرف,الاسم,كود الطالب,الصف,الحالة");
        
        foreach (var s in students)
        {
            var status = s.IsActive ? "نشط" : "غير نشط"; // Active : Inactive
            builder.AppendLine($"{s.Id},{EscapeCsv(s.Name)},{s.StudentCode},{EscapeCsv(s.Grade)},{status}");
        }

        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"students_export_{DateTime.Now:yyyyMMdd}.csv");
    }

    private string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
