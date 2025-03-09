using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class HardExitCommand: IBotCommand
{
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,
        string[] args)
    {
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "EXECUTE ORDER 66.\n ",
            cancellationToken: cancellationToken);
        Environment.Exit(0);
    }
}