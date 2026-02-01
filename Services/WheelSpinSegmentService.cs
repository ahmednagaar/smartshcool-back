using AutoMapper;
using Nafes.API.DTOs.WheelGame;
using Nafes.API.Modules;
using Nafes.API.Repositories;

namespace Nafes.API.Services;

public interface IWheelSpinSegmentService
{
    Task<IEnumerable<WheelSpinSegment>> GetActiveSegmentsAsync();
    // Admin methods could be added here (Create, Update, Delete)
}

public class WheelSpinSegmentService : IWheelSpinSegmentService
{
    private readonly IWheelSpinSegmentRepository _repository;

    public WheelSpinSegmentService(IWheelSpinSegmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<WheelSpinSegment>> GetActiveSegmentsAsync()
    {
        return await _repository.GetActiveSegmentsAsync();
    }
}
