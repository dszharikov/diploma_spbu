namespace DatasetCollector.Services.MLSerializerServices;

public class MLSerializerService : IMLSerializerService
{
    private readonly HttpClient _httpClient;

    public MLSerializerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> NotifyMLSerializer()
    {
        var uri = _httpClient.BaseAddress + "/refit";

        var response = await _httpClient.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            return Results.Ok(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return Results.BadRequest();
        }

    }
}