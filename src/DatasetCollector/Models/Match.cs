namespace DatasetCollector.Models;

public class Match
{
    public long MatchId { get; set; }
    public bool RadiantWin { get; set; }
    public long StartTime { get; set; }
    public int Duration { get; set; }
    public string RadiantTeam { get; set; }
    public string DireTeam { get; set; }
    public int? AverageMMR { get; set; }
}