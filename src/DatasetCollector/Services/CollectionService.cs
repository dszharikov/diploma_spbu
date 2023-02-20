using DatasetCollector.DataBases;
using DatasetCollector.Models;
using DatasetCollector.Parsers;

namespace DatasetCollector.Services;

public class CollectionService : BackgroundService
{
    private readonly IParser _parser;
    private readonly AppDbContext _context;
    private PeriodicTimer? _timer;

    public CollectionService(IParser parser, AppDbContext context)
    {
        _parser = parser;
        _context = context;
        _timer = null;
        //_dailyTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (_context.Matches.Any())
            {
                var maxMatchId = _context.Matches.Max(match => match.MatchId);
                var matches = await _parser.GetMatches();
                if (matches.Count > 0)
                {
                    // checking if there is NO MATCH with MATCH ID LESS than MAX MATCH ID IN DB
                    while (matches.FirstOrDefault(m => m.MatchId <= maxMatchId) is null)
                    {
                        _context.Matches.AddRange(matches);
                        matches = await _parser.GetMatches(lessThanMatchId: matches[^1].MatchId);
                        Thread.Sleep(Constants.IntervalBetweenRequestsInMilliSec);
                    }
                    _context.Matches.AddRange(matches.Where(m => m.MatchId > maxMatchId));
                    await _context.SaveChangesAsync(stoppingToken);
                }
            }
            else
            {
                await CollectAllMatchesCurrentPatch();
            }


            _timer ??= new PeriodicTimer(TimeSpan.FromMinutes(1));
        } while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
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

        await _context.Matches.AddRangeAsync(matches.Where(m => m.StartTime > Constants.UnixTimeLastPatch));
    }
}