using ConsoleApp1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class CookieCommand : IBotCommand
{
    private readonly AppSettingRuntimeChangerService _appSettingRuntimeChangerService;
    
    public CookieCommand(AppSettingRuntimeChangerService appSettingRuntimeChangerService)
    {
        _appSettingRuntimeChangerService = appSettingRuntimeChangerService;
    }
    private void ValidateUserParams(string[] args)
    {
        if (args.Length < 2)
        {
            throw new ApplicationException("Invalid cookie.");
        }
    }
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        ValidateUserParams(args);
        var cookie = string.Join(' ', args);
        _appSettingRuntimeChangerService.SetCookie(cookie);
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Cookie was set.\nIf bot fails check token.", 
        cancellationToken: cancellationToken);

    }
}