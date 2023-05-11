namespace DatasetCollector.Models;

public class ProMatch
{
    public long match_id { get; set; }
    public int duration { get; set; }
    public long start_time { get; set; }
    public int? radiant_team_id { get; set; }
    public string? radiant_name { get; set; }
    public int? dire_team_id { get; set; }
    public string? dire_name { get; set; }
    public int leagueid { get; set; }
    public string? league_name { get; set; }
    public int series_id { get; set; }
    public int series_type { get; set; }
    public int radiant_score { get; set; }
    public int dire_score { get; set; }
    public bool radiant_win { get; set; }
}