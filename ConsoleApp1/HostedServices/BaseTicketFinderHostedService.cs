using System.Runtime.InteropServices;
using ConsoleApp1.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Security.Authentication;
using ConsoleApp1.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

namespace ConsoleApp1.HostedServices
{
    public abstract class BaseTicketFinderHostedService : BackgroundService, IHealthCheck
    {
        private readonly TicketService _tickerService;
        private readonly AppSettingRuntimeChangerService _appSettingRuntimeChangerService;
        private readonly TelegramBotService _botClient;
        private readonly IServiceProvider _serviceProvider;

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


        protected BaseTicketFinderHostedService(TicketService officeGovClient, AppSettingRuntimeChangerService appSettingRuntimeChangerService, 
            TelegramBotService botClient, IServiceProvider serviceProvider)
        {
            _tickerService = officeGovClient;
            _appSettingRuntimeChangerService = appSettingRuntimeChangerService;
            _botClient = botClient;
            _serviceProvider = serviceProvider;
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
                            using var scope = _serviceProvider.CreateScope();
                            var _applicationRules = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ApplicationRules>>().Value;
                            var startSearchingDate = _applicationRules.StartSearchingDate ?? DateTime.Now;
                            _applicationRules.StartSearchingDate = startSearchingDate;
                            await ExecuteInLoopAsync(stoppingToken, _applicationRules ?? new ApplicationRules(), scope);
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

        public virtual async Task ExecuteInLoopAsync(CancellationToken stoppingToken, ApplicationRules startSearchingDate, IServiceScope scope)
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
                await _tickerService.GetTicketWithDelay(DateTime.Now, cancellationToken);
                return HealthCheckResult.Healthy("Service is running.");
            }
            catch (InvalidCredentialException)
            {
                return HealthCheckResult.Unhealthy("Service is stopped.");
            }

        }
    }
}
