using AutoMapper;
using Nafes.API.DTOs.Student;
using Nafes.API.DTOs.Question;
using Nafes.API.DTOs.Game;
using Nafes.API.DTOs.TestResult;
using Nafes.API.Modules;

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
    }
}

