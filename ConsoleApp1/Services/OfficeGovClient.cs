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
    public async Task<List<ApiResponce>> GetTicket(Dictionary<string,string> formData, CancellationToken cs)
    {
        var fullResponse = await _getTicket(formData, cs);
        if (fullResponse.StatusCode >= HttpStatusCode.InternalServerError)
        {
            throw new ExternalException($"Remote server is not working. StatusCode: {fullResponse.StatusCode}");
        }
        var strResponse = await fullResponse.Content.ReadAsStringAsync(cs);
        if (string.IsNullOrEmpty(strResponse) || !fullResponse.IsSuccessStatusCode)
        {
            throw new InvalidCredentialException($"Your credentials is outdated.");
        }

        return new List<ApiResponce>();
    }
    private async Task<HttpResponseMessage> _getTicket(Dictionary<string, string> formData, CancellationToken cs)
    {
        var content = new FormUrlEncodedContent(formData);
        return await _client.PostAsync("/site/freetimes", content, cs);
    }
}