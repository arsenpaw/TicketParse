using ConsoleApp1.Config;
using ConsoleApp1.Constants;
using ConsoleApp1.Dto;
using Microsoft.Extensions.Options;

namespace ConsoleApp1.Services;

public class TicketService
{

    private readonly OfficeGovClient _client;
    
    public TicketService(OfficeGovClient client)
    {
        _client = client;
    }

    public async Task<GovTicketResponse> GetTicketWithDelay(DateTime searchDate, int officeId, CancellationToken stoppingToken)
    {
        var formData = new Dictionary<string, string>
        {
            { "office_id", officeId.ToString()},
            { "date_of_admission", searchDate.ToString(GovApiConstants.DateFormat)},
            { "question_id", "55" }
        };
        await Task.Delay(TimeSpan.FromMilliseconds(800), stoppingToken);
        return await _client.GetTicket(formData, stoppingToken);
    }
}