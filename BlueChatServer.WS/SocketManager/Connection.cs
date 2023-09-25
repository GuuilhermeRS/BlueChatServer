using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace BlueChatServer.WS.SocketManager;

public class Connection
{
    private ConcurrentDictionary<string, WebSocket> _connections = new();

    public ConcurrentDictionary<string, WebSocket> ListConnections()
    {
        return _connections;
    }
    
    public void AddConnection(string id, WebSocket socket)
    {
        _connections.TryAdd(id, socket);
    }
    
    public WebSocket GetConnection(string id)
    {
        return _connections
            .FirstOrDefault(s => s.Key == id).Value;
    }

    public string GetSocketId(WebSocket socket)
    {
        return _connections
            .FirstOrDefault(s => s.Value == socket).Key;
    }

    public async Task RemoveConnection(string id)
    {
        _connections.TryRemove(id, out var socket);
        await socket!.CloseAsync(WebSocketCloseStatus.NormalClosure, 
            "Encerrando a conexão websocket", CancellationToken.None);
    }
}