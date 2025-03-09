using System.Security.Authentication;
using ConsoleApp1.Services;
using Serilog.Core;
using Serilog.Events;

namespace ConsoleApp1.Utils;

public class CriticalExceptionSink : ILogEventSink
{
    private readonly IFormatProvider _formatProvider;
    private readonly TelegramBotService _telegramBotService;

    public CriticalExceptionSink(TelegramBotService telegramBotService, IFormatProvider formatProvider = null)
    {
        _telegramBotService = telegramBotService;
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level == LogEventLevel.Fatal)
        {
            _telegramBotService.SendMessageToAll($"Attention! Critical error: {logEvent?.Exception?.Message}").Wait();
            if (logEvent.Exception is InvalidCredentialException)
            {
                _telegramBotService.SendMessageToAll($"Youre service is stopped until you change credentials using:\n/rsftoken <i>param</i>\n/cookie <i>param</i>").Wait();
               
            }
        }
    }
}
