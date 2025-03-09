using ConsoleApp1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Commands;

public class StartCommand : IBotCommand
{
    private readonly UserInfoService _userInfoService;

    public StartCommand(UserInfoService userInfoService)
    {
        _userInfoService = userInfoService;
    }
    
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Select office", "Check" }
        });
        await _userInfoService.AddUser(message.Chat.Id, new List<int>());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Welcome! You can select service center and receive their tickets.\n",
            replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken);
    }
}