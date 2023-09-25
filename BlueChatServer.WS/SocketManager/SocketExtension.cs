using System.Reflection;

namespace BlueChatServer.WS.SocketManager;

public static class SocketExtension
{
    public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
    {
        services.AddTransient<Connection>();
        foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
        {
            if (type.GetTypeInfo().BaseType == typeof(SocketHandler))
                services.AddSingleton(type);
        }

        return services;
    }

    public static IApplicationBuilder MapSockets(this IApplicationBuilder app, PathString path, SocketHandler? socket)
    {
        return app.Map(path, s => s.UseMiddleware<SocketMiddleware>(socket));
    }
}