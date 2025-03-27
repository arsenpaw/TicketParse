using ConsoleApp1.Constants;
using ConsoleApp1.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace ConsoleApp1.Services;

public class OfficeGovClient
{
    private readonly HttpClient _client;
    private readonly ILogger<OfficeGovClient> _logger;
    
    public OfficeGovClient(HttpClient client, ILogger<OfficeGovClient> logger)
    {
        _client = client;
        _logger = logger;
    }
    /// <exception cref="ExternalException"></exception>
    /// <exception cref="InvalidCredentialException"></exception>
    public async Task<List<ApiResponce>> GetTicket(DateTime date, CancellationToken cs)
    {
        var fullResponse = await _getTicket(date, cs);
        if (fullResponse.StatusCode >= HttpStatusCode.InternalServerError)
        {
            throw new ExternalException($"Remote server is not working. StatusCode: {fullResponse.StatusCode}");
        }
        var strResponse = await fullResponse.Content.ReadAsStringAsync(cs);
        if (string.IsNullOrEmpty(strResponse) || !fullResponse.IsSuccessStatusCode)
        {
            throw new InvalidCredentialException($"Your credentials is outdated.");
        }
        return JsonConvert.DeserializeObject<List<ApiResponce>>(strResponse)!;
    }
    private async Task<HttpResponseMessage> _getTicket(DateTime dateTime, CancellationToken cs)
    {
        return await _client.GetAsync($"/api/v2/departments?serviceId=49&date={dateTime.ToString("yyyy-MM-dd")}", cs);
    }
}