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
    private readonly ILogger<TicketFinderTicketFinderHostedService> _logger;
    
    private readonly AsyncRetryPolicy<List<Ticket>> retryPolicy = Policy<List<Ticket>>
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
        :base(tickerService,appSettingRuntimeChangerService, botClient, serviceProvider)
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;

    }
    public override async Task ExecuteInLoopAsync(CancellationToken stoppingToken, ApplicationRules _applicationRules, IServiceScope scope)
    {
       var  startSearchingDate = _applicationRules.StartSearchingDate ?? DateTime.Now;
            for (int i = 0; i < _applicationRules.DaysToSearch; i++)
            {
                startSearchingDate = startSearchingDate.AddDays(1);
                //to get new instance of service
                var tickerService = scope.ServiceProvider.GetRequiredService<TicketService>();
                var ticketList = await retryPolicy.ExecuteAsync(async () => await tickerService.GetTicketWithDelay(startSearchingDate, stoppingToken));
                var targetTicketList = ticketList.Where(x => _applicationRules.OfficeIds.Contains(x.OfficeId)).ToList();
                if (!targetTicketList.Any())
                {
                    _logger.LogInformation("Ticket not found on date: " + startSearchingDate.ToShortDateString());
                    continue;
                }

                foreach (var ticket in targetTicketList)
                {
                    var foundMsg = $"Ticket found on date: {startSearchingDate.ToShortDateString()}\n " +
                                   $"On time {string.Join(" ", ticket.Time)}\n" +
                                   $"Go to https://eq.hsc.gov.ua/site/step1?value=55\n" +
                                   $"{ticket.Location}";
                    _logger.LogWarning(foundMsg);
                    await Task.Run(async () => await _botClient.SendMessageToSubscribers(foundMsg, ticket.OfficeId), stoppingToken);
                }
        }
            _logger.LogInformation("Wait for {p1} minute(s);", TimeSpan.FromMinutes(_applicationRules.MinutesRequestDelay));

        await Task.Delay(TimeSpan.FromMinutes(_applicationRules.MinutesRequestDelay), stoppingToken);

    }
}