using AutoMapper;
using DatasetCollector.Models;
using DatasetCollector.Parsers;
using DatasetCollector.Profiles;

namespace DatasetCollector;

[TestFixture]
public class OpenDotaGetMatchesTest
{
    private OpenDotaParser _openDotaParser;
    private List<Match>? _matches;
    [SetUp]
    public void Setup()
    {
        var mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<MatchProfile>()));
        _openDotaParser = new OpenDotaParser(mapper);
        
        _matches = _openDotaParser.GetMatches().Result;
    }

    [Test]
    public void NotNullRequestTest()
    {
        Assert.False(_matches is null);
    }

    [Test]
    public void NoRepeatedMatchesTest()
    {
        Assert.False(AreRepeatedMatches(_matches));
    }

    [Test]
    public void NotNullLessThanMatchIdRequest()
    {
        var lessThanMatchId = _matches.Min(m => m.MatchId);
        var matches = _openDotaParser.GetMatches(lessThanMatchId).Result;
        Assert.False(matches is null);
    }

    [Test]
    public void NoRepeatedMatchesLessThanMatchId()
    {
        var minimalMatchId = _matches.Min(m => m.MatchId);
        var matches = _openDotaParser.GetMatches(minimalMatchId).Result;

        var firstMatchInRequest = _matches.FirstOrDefault(match => match.MatchId == minimalMatchId);
        
        matches.Add(firstMatchInRequest);
        
        Assert.False(AreRepeatedMatches(matches));
    }

    private bool AreRepeatedMatches(List<Match>? matches)
    {
        if (matches is null)
        {
            return true;
        }
        
        for (var i = 0; i < matches.Count; i++)
        {
            for (var j = 0; j < matches.Count; j++)
            {
                if (matches[i].MatchId == matches[j].MatchId && i != j)
                {
                    return true;
                }
            }
        }

        return false;
    }
}