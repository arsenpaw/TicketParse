using ConsoleApp1.Config;
using ConsoleApp1.Extensions;
using ConsoleApp1.HostedServices;
using ConsoleApp1.Services;
using ConsoleApp1.Storage;
using ConsoleApp1.Utils;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var builder = Host.CreateApplicationBuilder();
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
var configuration = configurationBuilder.Build(); 
builder.AddServiceDefaults();
builder.Configuration.AddConfiguration(configuration);
builder.Services.AddSingleton(configuration); // Ensure it's registered properly
builder.Services.AddHealthChecks()
        .AddCheck<TicketFinderTicketFinderHostedService>("CheckHealthAsync");
builder.Services.AddCommands();
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddScoped<OfficeGovClient>();
builder.Services.AddSingleton<CriticalExceptionSink>();
builder.Services.AddSingleton<AppSettingRuntimeChangerService>();
builder.Services.AddHostedService<TicketFinderTicketFinderHostedService>();
builder.Services.AddSingleton<UserInfoService>();
builder.Services.Configure<ApplicationCredentials>(builder.Configuration.GetSection("Credentials"));
builder.Services.Configure<ApplicationRules>(builder.Configuration.GetSection("ApplicationRules"));
builder.Services.AddScoped<TicketService>();
builder.Services.AddHttpClient<OfficeGovClient>(client =>
{
    using var serviceProvider = builder.Services.BuildServiceProvider();
    var appCredentials = serviceProvider.GetRequiredService<IOptionsSnapshot<ApplicationCredentials>>().Value;

    client.BaseAddress = new Uri("https://eq.hsc.gov.ua/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd(appCredentials.UserAgent);
    client.DefaultRequestHeaders.Add("x-csrf-token", appCredentials.XCsrfToken);
    client.DefaultRequestHeaders.Add("cookie", appCredentials.Cookie);
    client.DefaultRequestHeaders.Add("x-requested-with", appCredentials.XRequestedWith);
    client.DefaultRequestHeaders.Add("referer", appCredentials.Referer);
});
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(provider =>
{
    using var serviceProvider = builder.Services.BuildServiceProvider();
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var botId = config.GetValue<string>("BotId");
    return new TelegramBotClient(botId);
});
builder.Services.AddSingleton<LiteDatabase>(provider =>
{
    return new LiteDatabase("Filename=storage.db; Connection=Shared");
});

builder.Services.AddSingleton<UserInfoRepository>();
builder.Services.AddSingleton<CriticalExceptionSink>();

builder.Services.AddSerilog((context, configuration) =>
{
    using var serviceProvider = builder.Services.BuildServiceProvider();
    var criticalSink = serviceProvider.GetRequiredService<CriticalExceptionSink>();
    
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt"), LogEventLevel.Warning)
        .WriteTo.Sink(criticalSink)
        .Enrich.FromLogContext();
});
var app = builder.Build();
var service = app.Services.GetRequiredService<TelegramBotService>();
service.StartReceivingAsync();
await app.RunAsync();
