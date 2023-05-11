using AutoMapper;
using DatasetCollector.Dtos;
using DatasetCollector.Models;
using Newtonsoft.Json;

namespace DatasetCollector.Parsers;

public class OpenDotaParser : IParser
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public OpenDotaParser(IMapper mapper, HttpClient httpClient)
    {
        _mapper = mapper;
        _httpClient = httpClient;
    }

    public async Task<List<Match>> GetMatches(long? lessThanMatchId = null)
    {

        string url = _httpClient.BaseAddress + "/publicMatches";
        if (lessThanMatchId == null)
        {
            lessThanMatchId = Constants.BiggestMatchIdLastPatch;
        }

        url += $"?less_than_match_id={lessThanMatchId}";
        var requestResult = await _httpClient.GetStringAsync(url);

        var matchesDto = JsonConvert.DeserializeObject<List<MatchParseDto>>(requestResult);

        return _mapper.Map<List<MatchParseDto>, List<Match>>(matchesDto);
    }

    public async Task<List<ProMatch>> GetProMatches(long? lessThanMatchId = null)
    {

        string url = _httpClient.BaseAddress + "/proMatches";
        if (lessThanMatchId is not null)
        {
            url += $"?less_than_match_id={lessThanMatchId}";
        }
        System.Console.WriteLine($"--> Sending request to {url}");
        var requestResult = await _httpClient.GetStringAsync(url);

        var proMatches = JsonConvert.DeserializeObject<List<ProMatch>>(requestResult);

        return proMatches;
    }
}