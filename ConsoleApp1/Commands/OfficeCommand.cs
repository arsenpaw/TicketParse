using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Commands;

public class OfficeCommand: IBotCommand
{
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {    //TODO Hardcoded values
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "СЦ на Богданівській", callbackData: "62"),
                InlineKeyboardButton.WithCallbackData(text: "СЦ на Апостола", callbackData: "61")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Обидва", callbackData: "0")
            }
            
        });
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Welcome! You can select service center and receive their tickets.\n",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}