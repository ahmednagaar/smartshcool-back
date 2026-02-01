using AutoMapper;
using Nafes.API.DTOs.Student;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Game;
using Nafes.API.DTOs.TestResult;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using System.Text.Json;

namespace Nafes.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student mappings
        CreateMap<Student, StudentGetDto>();
        CreateMap<StudentCreateDto, Student>();
        CreateMap<StudentUpdateDto, Student>();

        // Question mappings
        CreateMap<Question, QuestionGetDto>()
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.DifficultyName, opt => opt.MapFrom(src => src.Difficulty.ToString()));
        CreateMap<QuestionCreateDto, Question>();
        CreateMap<QuestionUpdateDto, Question>();

        // Game mappings
        CreateMap<Game, GameGetDto>()
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.GameQuestions.Count));
        
        CreateMap<Game, GameWithQuestionsDto>()
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.GameQuestions.OrderBy(gq => gq.Order)));
        
        CreateMap<GameQuestion, GameQuestionDto>()
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Question.Text))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type.ToString()))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Question.Difficulty.ToString()))
            .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => src.Question.MediaUrl))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Question.Options));

        // TestResult mappings
        CreateMap<TestResult, TestResultGetDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.Name))
            .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title));
        
        CreateMap<TestResult, TestResultDetailDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.Name))
            .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title));
        
        CreateMap<TestResultCreateDto, TestResult>();
        
        // WheelQuestion mappings
        CreateMap<WheelQuestion, WheelQuestionResponseDto>()
            .ForMember(dest => dest.GradeId, opt => opt.MapFrom(src => (int)src.GradeId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => (int)src.SubjectId))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => BuildShuffledOptions(src.CorrectAnswer, src.WrongAnswers)));
        
        // CreateWheelQuestionDto -> WheelQuestion
        CreateMap<CreateWheelQuestionDto, WheelQuestion>()
            .ForMember(dest => dest.GradeId, opt => opt.MapFrom(src => (GradeLevel)src.GradeId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => (SubjectType)src.SubjectId))
            .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel))
            .ForMember(dest => dest.TestType, opt => opt.MapFrom(src => src.TestType ?? TestType.Nafes))
            .ForMember(dest => dest.WrongAnswers, opt => opt.MapFrom(src => SerializeWrongAnswers(src.WrongAnswers)))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
        
        // UpdateWheelQuestionDto -> WheelQuestion
        CreateMap<UpdateWheelQuestionDto, WheelQuestion>()
            .ForMember(dest => dest.GradeId, opt => opt.MapFrom(src => (GradeLevel)src.GradeId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => (SubjectType)src.SubjectId))
            .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel))
            .ForMember(dest => dest.TestType, opt => opt.MapFrom(src => src.TestType ?? TestType.Nafes))
            .ForMember(dest => dest.WrongAnswers, opt => opt.MapFrom(src => SerializeWrongAnswers(src.WrongAnswers)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

    private static string SerializeWrongAnswers(List<string> wrongAnswers)
    {
        return JsonSerializer.Serialize(wrongAnswers ?? new List<string>());
    }

    private static List<string> BuildShuffledOptions(string correctAnswer, string wrongAnswersJson)
    {
        var options = new List<string> { correctAnswer };
        try
        {
            var wrongAnswers = JsonSerializer.Deserialize<List<string>>(wrongAnswersJson ?? "[]");
            if (wrongAnswers != null)
                options.AddRange(wrongAnswers);
        }
        catch { /* Ignore parse errors */ }
        
        // Shuffle
        var rng = new Random();
        return options.OrderBy(_ => rng.Next()).ToList();
    }
}

