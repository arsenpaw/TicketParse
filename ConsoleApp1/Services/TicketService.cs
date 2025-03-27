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

    public async Task<List<Ticket>> GetTicketWithDelay(DateTime searchDate, CancellationToken stoppingToken)
    {
        var formData = new Dictionary<string, string>
        {
            { "date", searchDate.ToString(GovApiConstants.DateFormat)},
            { "serviceId:", "49" }
        };
        await Task.Delay(TimeSpan.FromMilliseconds(800), stoppingToken);
        var responce =  await _client.GetTicket(searchDate, stoppingToken);
        return responce.Select(x => 
            new Ticket{ Time = searchDate, Location = $"{x.SrvCenterName} на {x.Street}", OfficeId = x.SrvCenterId}).ToList();
    }
}