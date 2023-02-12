namespace DatasetCollector.Dtos;

public class MatchParseDto
{
    public long match_id { get; set; }
    public long match_seq_num { get; set; }
    public bool radiant_win { get; set; }
    public long start_time { get; set; }
    public int duration { get; set; }
    public string radiant_team { get; set; }
    public string dire_team { get; set; }
    public int? avg_mmr { get; set; }
}