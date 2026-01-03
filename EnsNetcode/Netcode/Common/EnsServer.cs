using ProtocolWrapper;
using ProtocolWrapper.Protocols.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Utils;

public class EnsServer
{
    internal static EnsServer Instance;

    internal Dictionary<int, EnsConnection> ClientConnections;
    private static List<int>ToRemove= new List<int>();
    internal EnsRoomManager RoomManager;
    internal ListenerBase Listener;

    public bool On;

    public EnsServer(IPAddress ip, int port)
    {
        Instance = this;

        ClientConnections = new Dictionary<int, EnsConnection>();
        Protocol.OnRecvConnection = (conn, index) => OnRecvConnection(conn, index);

        Listener = Protocol.GetListener(ip, port);
        RoomManager = new EnsRoomManager();
        On = true;
    }
    internal virtual void OnRecvConnection(ProtocolBase conn, short index)
    {
        EnsConnection connection = new EnsConnection(conn, index,OnConnectionShutDown);
        ClientConnections.Add(index, connection);
    }
    internal void OnConnectionShutDown(EnsConnection conn)
    {
        if (!On) return;//房间管理器关闭时不会发送退出房间
        if (conn.room != null) conn.room.Exit(conn);
        ClientConnections.Remove(conn.ClientId);
    }
    public void StartListening()
    {
        if (!Listener.Listening) Listener.StartListening();
    }
    public void EndListening()
    {
        if (Listener.Listening) Listener.EndListening();
    }
    internal void Update()
    {
        foreach (var kvp in ClientConnections)
        {
            var i=kvp.Value;
            if (Time.time > i.hbRecvTime)
            {
                i.ShutDown();
                ToRemove.Add(kvp.Key);
                continue;
            }
            if (Time.time > i.hbSendTime)
            {
                t_connection = i;
                i.Send(Header.H, SendTo.Server, SendTo.To(i.ClientId), Delivery.Unreliable, HeartBeatWriter);
                i.hbSendTime = Time.time + EnsInstance.HeartbeatMsgInterval;
            }
            i.Update();
        }
        if (ToRemove.Count > 0)
        {
            foreach (var i in ToRemove) ClientConnections.Remove(i);
            ToRemove.Clear();
        }
    }
    internal void FlushSendBuffer()
    {
        foreach (var i in ClientConnections.Values) i.FlushSendBuffer();
    }
    private static EnsConnection t_connection;
    private static bool HeartBeatWriter(SendBuffer buffer)
    {
        return IntSerializer.Serialize((int)(Utils.Time.time * 1000), buffer.bytes, ref buffer.indexStart)
            && IntSerializer.Serialize(t_connection.delay, buffer.bytes, ref buffer.indexStart);
    }
    public virtual void ShutDown()
    {
        if (!On) return;
        On = false;
        EndListening();
        Listener.ShutDown();
        Listener.Dispose();
        RoomManager.ShutDown();
        foreach (var i in ClientConnections.Values) i.ShutDown();
        ClientConnections.Clear();
        ClientConnections = null;
        Listener = null;
        RoomManager = null;
        Instance = null;
    }
}
