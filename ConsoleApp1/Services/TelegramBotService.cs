using ConsoleApp1.Command;
using ConsoleApp1.Commands;
using ConsoleApp1.Commands.KeyboardCommand;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Services
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient _botClient;
        private CancellationTokenSource _cts;
        private readonly UserInfoService _userInfoService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<string, IBotCommand> _botCommandResolver;
        public TelegramBotService(UserInfoService userInfoService, ITelegramBotClient botClient, IServiceProvider serviceProvider, Func<string, IBotCommand> botCommandResolver, OfficeCallbackCommand officeCallbackCommand)
        {
            _userInfoService = userInfoService;
            _botClient = botClient;
            _serviceProvider = serviceProvider;
            _botCommandResolver = botCommandResolver;
        }
        
        public void StartReceivingAsync()
        {

            _cts = new CancellationTokenSource();
            _botClient.StartReceiving(
               updateHandler: HandleUpdateAsync,
               errorHandler: HandleErrorAsync,
               cancellationToken: _cts.Token);
        }
        
        public async Task StopReceiving()
        {
            await this.SendMessageToAll("Bot stop working");
            _cts?.Cancel();
        }
        
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {  //Todo Bad way to handle this, should be refactored
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessage(botClient, update, cancellationToken);
                        break;
                    case UpdateType.CallbackQuery:
                        await HandleCallback(botClient, update, cancellationToken);
                        break;
                    default:
                        throw new ApplicationException($"Unsupported update type: {update.Type}");
                }
            }
            catch (ApplicationException e)
            {
                await botClient.SendMessage(
                    chatId: update?.Message?.Chat.Id,
                    text: e.Message,
                    cancellationToken: cancellationToken);
            }
            
        }   
        private async Task HandleMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {          
            var message = update.Message?.Text;
            var args = message?.Split(' ').Where(x => !x.Contains('/')).ToArray() ?? [];
            if (message?.FirstOrDefault() != '/')
            {
                message = KeyboardCommandResolver.Resolve(message);
            }
            else
            {
                message = message.Split(' ').FirstOrDefault();
            }
            
            var command = _botCommandResolver(message);
            await command.ExecuteAsync(update?.Message, botClient, update, cancellationToken,args);
        }
        private async Task HandleCallback(ITelegramBotClient botClient, Update update, CancellationToken cancellationToke)
        {
           await _botCommandResolver(CommandType.OfficeCallback).ExecuteAsync(update?.CallbackQuery.Message, botClient, update, cancellationToke, []);
        }
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            string errorMessage = exception switch
            {
                ApiRequestException apiEx =>
                    $"Telegram API Error:\n[{apiEx.ErrorCode}]\n{apiEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
        
        public async Task SendMessageToAll(string message)
        {
            foreach (var user in _userInfoService.GetAllUsers())
            {
                await _botClient.SendMessage(user.TelegramId, message, parseMode: ParseMode.Html);
            }
        }
        public async Task SendMessageToSubscribers(string message, int officeId)
        {
            foreach (var user in _userInfoService.GetAllUsers().Where(user => user.Offices.Contains(officeId)))
            {
                await _botClient.SendMessage(user.TelegramId, message);
            }
        }
        public async Task SendToConcreteUser(long chatId, string message)
        {
            await _botClient.SendMessage(chatId, message);
        }
    }
}