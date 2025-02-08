using System.Reflection;

namespace SchedulerService.Domain.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        var servicesTypes = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(s => s.Name.EndsWith("Service") && s.IsClass)
            .ToDictionary(
                s => s.ImplementedInterfaces.ElementAt(0),
                s => s
            );

        foreach (var service in servicesTypes)
            services.AddScoped(service.Key, service.Value);
        
        return services;
    }
}