using ConsoleApp1.Config;
using ConsoleApp1.Dto;
using ConsoleApp1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Serilog;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConsoleApp1.HostedServices;

public class TicketFinderTicketFinderHostedService : BaseTicketFinderHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramBotService _botClient;
    private   ApplicationRules _applicationRules;
    private readonly ILogger<TicketFinderTicketFinderHostedService> _logger;
    
    private readonly AsyncRetryPolicy<GovTicketResponse> retryPolicy = Policy<GovTicketResponse>
    .Handle<ExternalException>()
    .WaitAndRetryAsync(
         retryCount: 8,
         sleepDurationProvider: retryAttempt => TimeSpan.FromMinutes(Math.Pow(4, retryAttempt)),
         onRetry: (exception, timeSpan, retryCount, context) =>
         {
             Log.Warning("External server exception: {exception}, retry count: {retryCount}", exception, retryCount);
         });
    public TicketFinderTicketFinderHostedService(TelegramBotService botClient, TicketService tickerService, 
        ILogger<TicketFinderTicketFinderHostedService> logger, IServiceProvider serviceProvider, AppSettingRuntimeChangerService appSettingRuntimeChangerService)
        :base(tickerService,appSettingRuntimeChangerService, botClient)
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;

    }
    //TODO Hardcoded values
    private readonly Dictionary<int, string> _officeName = new Dictionary<int, string>()
    {
        { 61, "СЦ на Апостола" },
        { 177, "СЦ на Богданівській" }
    };

    public override async Task ExecuteInLoopAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope() ;
        _applicationRules = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ApplicationRules>>().Value;
        foreach (var officeId in _applicationRules.OfficeIds)
        {
            var startSearchingDate = _applicationRules.StartSearchingDate ?? DateTime.Now;
            startSearchingDate = startSearchingDate < DateTime.Now ? DateTime.Now : startSearchingDate;
            for (int i = 0; i < _applicationRules.DaysToSearch; i++)
            {
                startSearchingDate = startSearchingDate.AddDays(1);
                //to get new instance of service

                var tickerService = scope.ServiceProvider.GetRequiredService<TicketService>();
                var ticket = await retryPolicy.ExecuteAsync(async () => await tickerService.GetTicketWithDelay(startSearchingDate, officeId, stoppingToken));
                if (ticket.Rows.Count == 0)
                {
                    _logger.LogInformation("Ticket not found on date: " + startSearchingDate.ToShortDateString());
                    continue;
                }
                var timeStr = ticket.Rows.Select(x => x.Time).ToList();
                var foundMsg = $"Ticket found on date: {startSearchingDate.ToShortDateString()}\n " +
                                $"On time {string.Join(" ", timeStr)}\n" +
                                $"Go to https://eq.hsc.gov.ua/site/step1?value=55\n" +
                                $"Office: {_officeName[officeId]}";
                _logger.LogWarning(foundMsg);
                await Task.Run(async () => await _botClient.SendMessageToSubscribers(foundMsg, officeId), stoppingToken);
            }
        }
        
        _logger.LogInformation("Wait for {p1} minute(s);", TimeSpan.FromMinutes(_applicationRules.MinutesRequestDelay));

        await Task.Delay(TimeSpan.FromMinutes(_applicationRules.MinutesRequestDelay), stoppingToken);

    }
}