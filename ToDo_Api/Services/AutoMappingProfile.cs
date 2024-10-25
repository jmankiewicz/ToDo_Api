using AutoMapper;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;

namespace ToDo_Api.Services;

/// <summary>
/// Mapping profile for AutoMapper.
/// </summary>
public class AutoMappingProfile : Profile
{
    /// <summary>
    /// Prepared maps.
    /// </summary>
    public AutoMappingProfile()
    {
        CreateMap<CreateToDoDto, ToDo>();
        CreateMap<ToDo, ToDoDto>();
        CreateMap<UpdateToDoDto, ToDo>();
    }
}
