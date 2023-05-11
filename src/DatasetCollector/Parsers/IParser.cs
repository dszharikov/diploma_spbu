using DatasetCollector.Models;

namespace DatasetCollector.Parsers;

public interface IParser
{
    Task<List<Match>> GetMatches(long? lessThanMatchId = null);
    Task<List<ProMatch>> GetProMatches(long? lessThanMatchId = null);
}