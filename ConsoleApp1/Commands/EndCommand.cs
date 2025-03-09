using ConsoleApp1.Keyborads;
using ConsoleApp1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class EndCommand : IBotCommand
{
    private readonly UserInfoService _userInfoService;

    public EndCommand(UserInfoService userInfoService)
    {
        _userInfoService = userInfoService;
    }

    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        await _userInfoService.SetUserOffices(message.Chat.Id, new List<int>());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            replyMarkup: ReplyKeyboards.BaseHomeKeyboard,
            text: "Your account subscription was removed.\n ",
            cancellationToken: cancellationToken);

    }
}