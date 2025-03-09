using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public interface IBotCommand
{
    public Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args);
}