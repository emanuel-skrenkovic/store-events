using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.AspNet.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventSubscribers(this IServiceCollection services, Type assemblyType)
        {
            Assembly assembly = assemblyType.Assembly;

            var mappings =
                from type in assembly.GetTypes()
                    where !type.IsAbstract
                    where !type.IsGenericType
                from i in type.GetInterfaces()
                    where i.IsGenericType
                    where i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>) 
                select new { service = i, implementation = type };
            
            foreach (var mapping in mappings)
            {
                services.AddSingleton(mapping.service, c =>
                    ActivatorUtilities.CreateInstance(
                        c,
                        mapping.implementation));
            }
            
            return services;
        }
    }
}