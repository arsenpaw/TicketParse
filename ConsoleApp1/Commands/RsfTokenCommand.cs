using ConsoleApp1.Keyborads;
using ConsoleApp1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class RsfTokenCommand: IBotCommand
{
    private readonly AppSettingRuntimeChangerService _appSettingRuntimeChangerService;
    public RsfTokenCommand(AppSettingRuntimeChangerService appSettingRuntimeChangerService)
    {
        _appSettingRuntimeChangerService = appSettingRuntimeChangerService;
    }
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,
        string[] args)
    {
        ValidateUserParams(args);
        var cookie = args[0];
        _appSettingRuntimeChangerService.SetRsrfToken(cookie);
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "Token was set.\nIf bot fails check cookie.", 
            replyMarkup: ReplyKeyboards.RichHomeKeyboard,
            cancellationToken: cancellationToken);
    }

    private void ValidateUserParams(string[] args)
    { 
        if (args.Length != 1)
        {
            throw new ApplicationException("Invalid number of arguments. Expected 1 arguments.");
        }
    }
}