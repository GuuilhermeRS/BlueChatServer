using BlueChatServer.WS.Handlers;
using BlueChatServer.WS.SocketManager;

namespace BlueChatServer.WS;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddWebSocketManager();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseWebSockets();
        app.MapSockets("/connect", serviceProvider.GetService<WsServer>());
        app.UseStaticFiles();
    }
}