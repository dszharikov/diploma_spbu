using Coravel.Invocable;
using DatasetCollector.DataBases;
using DatasetCollector.Models;
using DatasetCollector.Parsers;
using DatasetCollector.Services.MLSerializerServices;

namespace DatasetCollector.Services;

public class DataCollector : IInvocable
{
    private IParser _parser;
    private readonly IMLSerializerService _mlSerliazerService;
    private AppDbContext _context;

    public DataCollector(AppDbContext context, IParser parser, IMLSerializerService mLSerializerService)
    {
        _context = context;
        _parser = parser;
        _mlSerliazerService = mLSerializerService;
    }

    public async Task Invoke()
    {
        Console.WriteLine($"----- Debug: Invoke executed {DateTime.Now}");
        if (_context.Matches.Any())
        {
            var maxMatchId = _context.Matches.Max(match => match.MatchId);
            await CollectMatchesNewerThanMatchId(maxMatchId);
            var minMatchId = _context.Matches.Min(match => match.MatchId);
            await CollectAllMatchesCurrentPatch(minMatchId);

            await _mlSerliazerService.NotifyMLSerializer();
        }
        else
        {
            await CollectAllMatchesCurrentPatch();
        }
        Console.WriteLine($"----- Debug: Invoke ended {DateTime.Now}");
    }
    
    private async Task CollectAllMatchesCurrentPatch(long? minimalMatchId = null)
    {
        List<Match> matches;
        if (minimalMatchId is not null)
        {
            matches = await _parser.GetMatches(lessThanMatchId: minimalMatchId);
        }
        else
        {
            matches = await _parser.GetMatches();
        }

        // checking if there is NO MATCH with START TIME LESS than DATE of CURRENT BIG UPDATE
        while (matches.FirstOrDefault(m => m.StartTime < Constants.UnixTimeLastPatchStarted) is null)
        {
            minimalMatchId = matches[^1].MatchId;
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
            matches = await _parser.GetMatches(lessThanMatchId: minimalMatchId); // ^1 = last element
            Thread.Sleep(Constants.IntervalBetweenRequestsInMilliSec);
        }

        await _context.Matches.AddRangeAsync(matches.Where(
            m => m.StartTime > Constants.UnixTimeLastPatchStarted));
    }

    private async Task CollectMatchesNewerThanMatchId(long maxMatchId)
    {
        var matches = await _parser.GetMatches();
        while (matches.FirstOrDefault(m => m.MatchId <= maxMatchId) is null)
        {
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();

            matches = await _parser.GetMatches(lessThanMatchId: matches[^1].MatchId);
            Thread.Sleep(Constants.IntervalBetweenRequestsInMilliSec);
        }

        _context.Matches.AddRange(matches.Where(m => m.MatchId > maxMatchId));
        await _context.SaveChangesAsync();
    }
}