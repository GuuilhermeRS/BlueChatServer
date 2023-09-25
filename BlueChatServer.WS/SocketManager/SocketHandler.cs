using System.Net.WebSockets;
using System.Text;

namespace BlueChatServer.WS.SocketManager;

public abstract class SocketHandler
{
    public Connection Connection { get; set; }

    public SocketHandler(Connection connection)
    {
        Connection = connection;
    }

    public virtual async Task OnConnected(string id, WebSocket socket)
    {
        await Task.Run(() => Connection.AddConnection(id, socket));
    }
    
    public virtual async Task OnDisconnected(WebSocket socket)
    {
        await Task.Run(() => Connection.RemoveConnection(Connection.GetSocketId(socket)));
    }

    public async Task SendMessage(WebSocket socket, string msg)
    {
        if (socket.State != WebSocketState.Open)
            return;

        var msgBytes = Encoding.ASCII.GetBytes(msg);
        var buffer = new ArraySegment<byte>(msgBytes, 0, msg.Length);
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task SendMessage(string id, string msg)
    {
        var con = Connection.GetConnection(id);
        await SendMessage(con, msg);
    }

    public async Task SendMessageToAll(string msg)
    {
        foreach (var con in Connection.ListConnections())
        {
            await SendMessage(con.Value, msg);
        }
    }

    public abstract Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
}