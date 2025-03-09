using ConsoleApp1.Config;
using ConsoleApp1.Keyborads;
using ConsoleApp1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class OfficeCallbackCommand : IBotCommand
{
    private readonly ApplicationRules _applicationRules;
    private readonly UserInfoService _userInfoService;


    public OfficeCallbackCommand(UserInfoService userInfoService, IOptionsMonitor<ApplicationRules> applicationRules)
    {
        _userInfoService = userInfoService;
        _applicationRules = applicationRules.CurrentValue;

    }
    //TODO Hardcoded values
    private List<int> _getOfficesToSend(int officeId)
    {
        var officeToSet = _applicationRules.OfficeIds.FirstOrDefault(x => x == officeId);
        if (officeToSet == 0)
        {
            return _applicationRules.OfficeIds;
        }
        return new List<int>() { officeToSet };
    }
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        var officeId = int.Parse(update.CallbackQuery.Data);
        var offisesToSend = _getOfficesToSend(officeId);
        await _userInfoService.SetUserOffices(message.Chat.Id, offisesToSend);
        await botClient.DeleteMessage(chatId: message.Chat.Id, messageId: message.MessageId, cancellationToken: cancellationToken);
        await botClient.SendMessage(message.Chat.Id,
            $"Offices set: {string.Join(' ', offisesToSend)}",
            replyMarkup: ReplyKeyboards.RichHomeKeyboard,
            cancellationToken: cancellationToken);

    }
}