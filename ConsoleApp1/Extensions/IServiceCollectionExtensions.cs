using ConsoleApp1.Command;
using ConsoleApp1.Commands;
using ConsoleApp1.ServiceConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1.Extensions;

public static class ServiceProviderExtensions
{
    public static void AddCommands(this IServiceCollection services)
    {
            services.AddBotCommandResolver<IBotCommand, string>(
                new Dictionary<string, Type>
            {
                {CommandType.StartCommand, typeof(StartCommand)},
                {CommandType.CheckCommand, typeof(CheckCommand)},
                {CommandType.EndCommand, typeof(EndCommand)},
                {CommandType.OfficeCommand, typeof(OfficeCommand)},
                {CommandType.OfficeCallback, typeof(OfficeCallbackCommand)},
                {CommandType.HomeCommand, typeof(HomeCommand)},
                {CommandType.CookieCommand, typeof(CookieCommand)},
                {CommandType.RsfToken, typeof(RsfTokenCommand)},
                {CommandType.HardExit, typeof(HardExitCommand)},
                {CommandType.Datetime, typeof(DateTimeCommand)}
                
                
                
            });
    }
}