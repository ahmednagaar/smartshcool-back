using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nafes.API.Data;
using Nafes.API.Repositories;
using Nafes.API.Repositories.FlipCard;
using Nafes.API.Services;
using Nafes.API.Services.FlipCard;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Register Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


// Matching Game
builder.Services.AddScoped<IMatchingGameRepository, MatchingGameRepository>();
builder.Services.AddScoped<IMatchingGameSessionRepository, MatchingGameSessionRepository>();
builder.Services.AddScoped<IMatchingGameService, MatchingGameService>();

// Wheel Game
builder.Services.AddScoped<IWheelQuestionRepository, WheelQuestionRepository>();
builder.Services.AddScoped<IWheelGameSessionRepository, WheelGameSessionRepository>();
builder.Services.AddScoped<IWheelQuestionAttemptRepository, WheelQuestionAttemptRepository>();
builder.Services.AddScoped<IWheelSpinSegmentRepository, WheelSpinSegmentRepository>();
builder.Services.AddScoped<IWheelQuestionService, WheelQuestionService>(); 
builder.Services.AddScoped<IWheelGameService, WheelGameService>();
builder.Services.AddScoped<IWheelSpinSegmentService, WheelSpinSegmentService>();

// Drag & Drop Game
builder.Services.AddScoped<IDragDropQuestionRepository, DragDropQuestionRepository>();
builder.Services.AddScoped<IDragDropGameSessionRepository, DragDropGameSessionRepository>();
builder.Services.AddScoped<IDragDropAttemptRepository, DragDropAttemptRepository>();
builder.Services.AddScoped<IUIThemeService, UIThemeService>();
builder.Services.AddScoped<IDragDropQuestionService, DragDropQuestionService>();
builder.Services.AddScoped<IDragDropGameService, DragDropGameService>();

// Flip Card Game
builder.Services.AddScoped<IFlipCardQuestionRepository, FlipCardQuestionRepository>();
builder.Services.AddScoped<IFlipCardGameSessionRepository, FlipCardGameSessionRepository>();
builder.Services.AddScoped<IFlipCardAttemptRepository, FlipCardAttemptRepository>();
builder.Services.AddScoped<IFlipCardQuestionService, FlipCardQuestionService>();
builder.Services.AddScoped<IFlipCardGameService, FlipCardGameService>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ISanitizationService, SanitizationService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

// Configure MailSettings
builder.Services.Configure<Nafes.API.Services.MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Register Hosted Services
// builder.Services.AddHostedService<DailyReportBackgroundService>();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NafesPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nafes API",
        Version = "v1",
        Description = "School Management & Testing Platform API"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nafes API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});

app.UseHttpsRedirection();

app.UseCors("NafesPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        DbSeeder.SeedAsync(context).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Database seeding failed: {ex.Message}");
        // Continue running the app even if seeding fails
    }
}

app.Run();

