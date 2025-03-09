using Microsoft.Extensions.Diagnostics.HealthChecks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class CheckCommand : IBotCommand
{
    private readonly HealthCheckService _healthCheckService;
    public CheckCommand(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        var healthCheckResult = await _healthCheckService.CheckHealthAsync(cancellationToken);
        var healthCheckDescription = healthCheckResult.Status == HealthStatus.Healthy ? "" : "Try to update credentials or restart bot.";
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: $"Bot status: {healthCheckResult.Status}\n{healthCheckDescription} ",
            cancellationToken: cancellationToken);
    }
}