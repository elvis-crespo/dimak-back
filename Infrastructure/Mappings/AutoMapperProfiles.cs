using AutoMapper;
using dimax_front.Core.Entities;
using dimax_front.Domain.DTOs;

namespace dimax_front.Infrastructure.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<InstallationHistoryDTO, InstallationHistory>();
        }
    }
}
