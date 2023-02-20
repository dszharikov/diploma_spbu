using AutoMapper;
using DatasetCollector.Dtos;
using DatasetCollector.Models;
using Newtonsoft.Json;

namespace DatasetCollector.Parsers;

public class OpenDotaParser : IParser
{
    private readonly string _originPath;
    private readonly IMapper _mapper;

    public OpenDotaParser(IMapper mapper, string originPath = Constants.OriginOpenDotaPath)
    {
        _originPath = originPath;
        _mapper = mapper;
    }

    public async Task<List<Match>> GetMatches(long? lessThanMatchId = null)
    {
        using var client = new HttpClient();
        
        string url = _originPath + "/publicMatches";
        if (lessThanMatchId != null)
        {
            url += $"?less_than_match_id={lessThanMatchId}";
        }
        
        var requestResult = await client.GetStringAsync(url);

        var matchesDto = JsonConvert.DeserializeObject<List<MatchParseDto>>(requestResult);

        return _mapper.Map<List<MatchParseDto>, List<Match>>(matchesDto);
    }
}