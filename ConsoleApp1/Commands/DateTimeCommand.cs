using ConsoleApp1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Commands;

public class DateTimeCommand: IBotCommand
{
    private readonly AppSettingRuntimeChangerService _appSettingRuntimeChangerService;
    
    public DateTimeCommand(AppSettingRuntimeChangerService appSettingRuntimeChangerService)
    {
        _appSettingRuntimeChangerService = appSettingRuntimeChangerService;
    }
    private int ValidateDayCount(string[] args)
    {
        var daySpan = args.FirstOrDefault();
        if (daySpan is null)
        {
            throw new ApplicationException("Invalid number of days.");
        }
        return Convert.ToInt32(daySpan);
    }
    private DateTime ValidateDateTime(string[] args)
    {
        if (args.Length < 2)
        {
            return DateTime.Now;
        }

        if (!DateTime.TryParse(args[1], out var time))
        {
            throw new ApplicationException("Invalid date format. Example: 2025-05-23.");
        }
        time = time < DateTime.Now ? DateTime.Now : time;
        return time;
    }
    public async Task ExecuteAsync(Message message, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string[] args)
    {
        var daysSpan =ValidateDayCount(args);
        var startDate = ValidateDateTime(args);
        _appSettingRuntimeChangerService.SetDateTime(daysSpan,startDate);
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: $"DateTime was set.\n{daysSpan} days, from {startDate.ToShortDateString()}", 
            cancellationToken: cancellationToken);

    }
}