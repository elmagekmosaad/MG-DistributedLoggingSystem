using AutoMapper;
using MGDistributedLoggingSystem.Constants;
using MGDistributedLoggingSystem.Data.Entities;
using MGDistributedLoggingSystem.Models;
using MGDistributedLoggingSystem.Models.Dtos.Auth;
using MGDistributedLoggingSystem.Models.Dtos.LogEntryDto;
namespace MGDistributedLoggingSystem.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LogEntry, LogEntryAddDto> ()
               .ReverseMap();

            CreateMap<LogEntry, LogEntryResponseDto>()
               .ReverseMap();

            CreateMap<LogEntryFilter, LogEntryFilterDto>()
               .ReverseMap();

            CreateMap<AppUser, RegisterDto>()
              .ReverseMap();

            CreateMap<AppUser, DefaultAdmin>()
              .ReverseMap();
            CreateMap<AppUser, DefaultUser>()
              .ReverseMap();
        }
    }
}
