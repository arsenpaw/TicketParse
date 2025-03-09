using System.Runtime.InteropServices;
using ConsoleApp1.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Security.Authentication;
using ConsoleApp1.Dto;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ConsoleApp1.HostedServices
{
    public abstract class BaseTicketFinderHostedService : BackgroundService, IHealthCheck
    {
        private readonly TicketService _tickerService;
        private readonly AppSettingRuntimeChangerService _appSettingRuntimeChangerService;
        private readonly TelegramBotService _botClient;

        private readonly AsyncCircuitBreakerPolicy policy = Policy
            .Handle<InvalidCredentialException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 1, 
                durationOfBreak: TimeSpan.MaxValue,
                onBreak: (ex, breakDelay) =>
                {
                    Log.Warning("Circuit breaker BREAK. Service is stopped.");
                },
                onReset: () =>
                {
                    Log.Information("Circuit breaker RESET. Service is back.");
                },
                onHalfOpen: () =>
                {
                    Log.Information("Circuit breaker HALF OPEN. Service is back.");
                }
            );


        protected BaseTicketFinderHostedService(TicketService officeGovClient, AppSettingRuntimeChangerService appSettingRuntimeChangerService, TelegramBotService botClient)
        {
            _tickerService = officeGovClient;
            _appSettingRuntimeChangerService = appSettingRuntimeChangerService;
            _botClient = botClient;
        }
        
        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _appSettingRuntimeChangerService.OnApplicationCredentialsChange += async () =>
            {
                policy.Reset();
            };
            await _botClient.SendMessageToAll("Ticket Service has stated searching tickets for you:).");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (policy.CircuitState == CircuitState.Closed)
                    {
                        await policy.ExecuteAsync(async () =>
                        {
                            await ExecuteInLoopAsync(stoppingToken);
                        });
                    }
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    Log.Information("Service is stopping.");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Unhandled exception occurred.");
                }
            }
        }

        public virtual async Task ExecuteInLoopAsync(CancellationToken stoppingToken)
        {
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _botClient.SendMessageToAll("Ticket Service stopped. Ticket will not be found !!!");
            await base.StopAsync(stoppingToken);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _tickerService.GetTicketWithDelay(DateTime.Now, 61, cancellationToken);
                return HealthCheckResult.Healthy("Service is running.");
            }
            catch (InvalidCredentialException)
            {
                return HealthCheckResult.Unhealthy("Service is stopped.");
            }

        }
    }
}
