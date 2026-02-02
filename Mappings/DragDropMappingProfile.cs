using AutoMapper;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.Modules;

namespace Nafes.API.Mappings;

public class DragDropMappingProfile : Profile
{
    public DragDropMappingProfile()
    {
        // Question Mappings
        CreateMap<DragDropQuestion, DragDropQuestionDto>()
            .ReverseMap();

        CreateMap<CreateDragDropQuestionDto, DragDropQuestion>();
        CreateMap<UpdateDragDropQuestionDto, DragDropQuestion>()
            .ForMember(dest => dest.Zones, opt => opt.Ignore()) // Handle manually in service for safety
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        // Zone Mappings
        CreateMap<DragDropZone, DragDropZoneDto>().ReverseMap();
        CreateMap<CreateDragDropZoneDto, DragDropZone>();
        CreateMap<UpdateDragDropZoneDto, DragDropZone>();

        // Item Mappings
        CreateMap<DragDropItem, DragDropItemDto>().ReverseMap();
        CreateMap<CreateDragDropItemDto, DragDropItem>();
        CreateMap<UpdateDragDropItemDto, DragDropItem>();
        
        // Game Session Mappings
        CreateMap<DragDropGameSession, GameSessionDto>()
            .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.DragDropQuestion.GameTitle))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.DragDropQuestion.Instructions))
            .ForMember(dest => dest.TimeLimit, opt => opt.MapFrom(src => src.DragDropQuestion.TimeLimit))
            .ForMember(dest => dest.ShowImmediateFeedback, opt => opt.MapFrom(src => src.DragDropQuestion.ShowImmediateFeedback))
            .ForMember(dest => dest.UITheme, opt => opt.MapFrom(src => src.DragDropQuestion.UITheme))
            .ForMember(dest => dest.Zones, opt => opt.MapFrom(src => src.DragDropQuestion.Zones))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DragDropQuestion.Items)); // Note: Service handles specific logic if needed
    }
}
