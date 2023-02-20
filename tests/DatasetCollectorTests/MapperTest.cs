using AutoMapper;
using DatasetCollector.Dtos;
using DatasetCollector.Models;
using DatasetCollector.Profiles;

namespace DatasetCollector;

[TestFixture]
public class MapperTest : TestCollectorBase
{
    [SetUp]
    public void Setup()
    {
         Mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<MatchProfile>()));
    }

    [Test]
    public void MapMatchParseDtoToMatchTest()
    {
        var matchParseDto = new MatchParseDto
        {
            match_id = long.MaxValue - 1,
            match_seq_num = long.MaxValue - 2,
            radiant_win = true,
            avg_mmr = int.MaxValue - 3,
            dire_team = "direTeam",
            radiant_team = "radiantTeam",
            duration = int.MaxValue - 4,
            start_time = long.MaxValue - 5
        };

        var match = Mapper.Map<MatchParseDto, Match>(matchParseDto);
        
        Assert.True(match.MatchId == matchParseDto.match_id
                    && match.RadiantWin == matchParseDto.radiant_win
                    && match.RadiantTeam == matchParseDto.radiant_team
                    && match.DireTeam == matchParseDto.dire_team
                    && match.Duration == matchParseDto.duration
                    && match.StartTime == matchParseDto.start_time
                    && match.AverageMMR == matchParseDto.avg_mmr);
    }

    [Test]
    public void MapNullMatchParseDtoToMatchTest()
    {
        MatchParseDto matchesParseDto = null;
        var match = Mapper.Map<MatchParseDto, Match>(matchesParseDto);
        
        Assert.True(match is null);
    }
}