using AutoMapper;
using DatasetCollector.Dtos;
using DatasetCollector.Models;

namespace DatasetCollector.Profiles;

public class MatchProfile : Profile
{
    public MatchProfile()
    {
        // Source -> Target
        CreateMap<MatchParseDto, Match>()
            .ForMember(dest => dest.MatchId,
                opt => opt.MapFrom(
                    src => src.match_id))
            .ForMember(dest => dest.RadiantWin,
                opt => opt.MapFrom(
                    src => src.radiant_win))
            .ForMember(dest => dest.RadiantTeam,
                opt => opt.MapFrom(
                    src => src.radiant_team))
            .ForMember(dest => dest.DireTeam,
                opt => opt.MapFrom(
                    src => src.dire_team))
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(
                    src => src.start_time))
            .ForMember(dest => dest.Duration,
                opt => opt.MapFrom(
                    src => src.duration))
            .ForMember(dest => dest.AverageMMR,
                opt => opt.MapFrom(
                    src => src.avg_mmr));
    }
}