using System.Net.WebSockets;
using System.Text;
using BlueChatServer.WS.SocketManager;

namespace BlueChatServer.WS.Handlers;

public class WsServer : SocketHandler
{
    public WsServer(Connection connection) : base(connection) { }

    public override async Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        var socketId = Connection.GetSocketId(socket);

        var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);

        await SendMessageToAll($"Mensagem recebida no websocket: {msg}");
    }
}