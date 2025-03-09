using ConsoleApp1.Keyborads;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class HomeCommand : IBotCommand
{
    public Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {

        return botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Hey! You can select service center and receive their tickets.\n",
            replyMarkup: ReplyKeyboards.RichHomeKeyboard,
            cancellationToken: cancellationToken);
    }
}