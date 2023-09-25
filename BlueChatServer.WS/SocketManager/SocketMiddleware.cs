using System.Net.WebSockets;

namespace BlueChatServer.WS.SocketManager;

public class SocketMiddleware
{
    private readonly RequestDelegate _next;
    public SocketHandler Handler { get; set; }

    public SocketMiddleware(RequestDelegate next, SocketHandler handler)
    {
        _next = next;
        Handler = handler;
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> messageHandler)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var r = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            messageHandler(r, buffer);
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return;

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await Handler.OnConnected(Guid.NewGuid().ToString(), socket);

        await Receive(socket, async (result, buffer) =>
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    await Handler.Receive(socket, result, buffer);
                    break;
                case WebSocketMessageType.Close:
                    await Handler.OnDisconnected(socket);
                    break;
                case WebSocketMessageType.Binary:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
    }
    
}