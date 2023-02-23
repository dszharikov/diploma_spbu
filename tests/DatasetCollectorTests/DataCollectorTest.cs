using DatasetCollector.Services;

namespace DatasetCollector;

[TestFixture]
public class DataCollectorTest : TestCollectorBase
{
    private DataCollector _dataCollector;

    [Test]
    public async Task NoWaitingBeforeExecuteTest()
    {
        Context = MatchContextFactory.Create();
        _dataCollector = new DataCollector(Context, Parser);
        
        var matches = await Parser.GetMatches();

        var medianMatchId = matches[matches.Count / 2].MatchId;

        var halfMatches = matches.Where(m => m.MatchId < medianMatchId).ToList();

        Context.Matches.AddRange(halfMatches);
        await Context.SaveChangesAsync();

        await _dataCollector.Invoke();

        Assert.True(Context.Matches.Count() > halfMatches.Count);
    }

    [Test]
    public async Task AddMoreThanOneMatchesPageTest()
    {
        Context = MatchContextFactory.Create();       
        _dataCollector = new DataCollector(Context, Parser);


        var matchesQuantity = 0;
        
        // getting third page of matches
        var matches = await Parser.GetMatches();
        matchesQuantity += matches.Count;
        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec);
        matches = await Parser.GetMatches(lessThanMatchId: matches[^1].MatchId);
        matchesQuantity += matches.Count;
        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec);
        matches = await Parser.GetMatches(lessThanMatchId: matches[^1].MatchId);
        matchesQuantity += matches.Count;
        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec);

        Context.Matches.AddRange(matches);
        await Context.SaveChangesAsync();

        await _dataCollector.Invoke();

        Assert.True(Context.Matches.Count() >= matchesQuantity);
    }
}