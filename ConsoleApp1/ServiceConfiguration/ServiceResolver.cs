namespace ConsoleApp1.ServiceConfiguration;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceResolversConfiguration
{
    public static void AddBotCommandResolver<
        TService,
        TServiceKeys
    >(this IServiceCollection services, Dictionary<TServiceKeys, Type> multiImplementedServices)
        where TService : class where TServiceKeys : notnull
    {
        foreach (var service in multiImplementedServices.Values)
        {
            services.AddScoped(service);
        }

        services.AddScoped<Func<TServiceKeys, TService>>(serviceProvider => serviceTypeName =>
        {
            var serviceType = multiImplementedServices.GetValueOrDefault(serviceTypeName);
            if (serviceType is null)
            {
                return null;
            }

            return serviceProvider.GetService(serviceType) as TService;
        });
    }
}
