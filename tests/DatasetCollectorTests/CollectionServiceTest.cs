using DatasetCollector.Services;

namespace DatasetCollector;

[TestFixture]
public class CollectionServiceTest : TestCollectorBase
{
    private CollectionService _collectionService;

    [SetUp]
    public void Setup()
    {
        _collectionService = new CollectionService(Parser, Context);
    }

    [Test]
    public async Task NoWaitingBeforeExecuteTest()
    {
        var matches = await Parser.GetMatches();

        var medianMatchId = matches[matches.Count / 2].MatchId;

        var halfMatches = matches.Where(m => m.MatchId < medianMatchId).ToList();
        
        Context.Matches.AddRange(halfMatches);
        await Context.SaveChangesAsync();

        await _collectionService.StartAsync(CancellationToken.None);

        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec * 2);

        await _collectionService.StopAsync(CancellationToken.None);
        Assert.True(Context.Matches.Count() > halfMatches.Count);
    }

    [Test]
    public async Task AddMoreThanOneMatchesPageTest()
    {
        Context.Matches.RemoveRange(Context.Matches);
        await Context.SaveChangesAsync();

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

        await _collectionService.StartAsync(CancellationToken.None);

        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec * 4);

        await _collectionService.StopAsync(CancellationToken.None);
        
        Assert.True(Context.Matches.Count() >= matchesQuantity);
    }

    [Test]
    public async Task CollectAllMatchesFromCurrentPatchTest()
    {
        Context.Matches.RemoveRange(Context.Matches);
        await Context.SaveChangesAsync();

        var cancellationTokenSource = new CancellationTokenSource();
        
        await _collectionService.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(Constants.IntervalBetweenRequestsInMilliSec * 5);

        cancellationTokenSource.Cancel();
        
        //await _collectionService.StopAsync(CancellationToken.None);
        
        Assert.True(Context.Matches.Any());
    }
}