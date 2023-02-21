using Coravel.Invocable;
using DatasetCollector.DataBases;
using DatasetCollector.Parsers;

namespace DatasetCollector.Services;

public class DataCollector : IInvocable
{
    private IParser _parser;
    private AppDbContext _context;

    public DataCollector(AppDbContext context, IParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public async Task Invoke()
    {
        if (_context.Matches.Any())
        {
            var maxMatchId = _context.Matches.Max(match => match.MatchId);
            await CollectMatchesNewerThanMatchId(maxMatchId);
        }
        else
        {
            await CollectAllMatchesCurrentPatch();
        }
    }
    
    private async Task CollectAllMatchesCurrentPatch()
    {
        var matches = await _parser.GetMatches();

        // checking if there is NO MATCH with START TIME LESS than DATE of CURRENT BIG UPDATE
        while (matches.FirstOrDefault(m => m.StartTime < Constants.UnixTimeLastPatch) is null)
        {
            await _context.Matches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
            matches = await _parser.GetMatches(lessThanMatchId: matches[^1].MatchId); // ^1 = last element
            Thread.Sleep(Constants.IntervalBetweenRequestsInMilliSec);
        }

        await _context.Matches.AddRangeAsync(matches.Where(
            m => m.StartTime > Constants.UnixTimeLastPatch));
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