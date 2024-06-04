using AutoMapper;
using Grpc.Core;
using PlatformService.Repositories;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;

    public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public override Task<PlatformResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var platforms = _repository.GetAllPlatforms();
        
        var response = new PlatformResponse
        {
            Platforms = { _mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms) }
        };

        return Task.FromResult(response);
    }
}