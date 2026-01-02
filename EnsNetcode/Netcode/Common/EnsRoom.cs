using System;
using System.Collections.Generic;

public class EnsRoom:Disposable
{
    public static EnsRoom Instance
    {
        get
        {
            if(EnsRoomManager.Instance == null)return null;
            if(EnsRoomManager.Instance.rooms.TryGetValue(EnsRoomManager.roomIdStart,out var room))return room;
            return null;
        }
    }
    private Dictionary<int,EnsConnection> ClientConnections =new Dictionary<int, EnsConnection>();
    public int RoomId;
    internal short CurrentAuthorityAt = -1;

    public Dictionary<string, (char, int)> Rule = new Dictionary<string, (char, int)>();
    public Dictionary<string,string>Info= new Dictionary<string, string>();

    // >0为游戏过程中制造的物体的Id
    private short createdid = 1;
    internal short CreatedId
    {
        get
        {
            return createdid;
        }
        set
        {
            createdid = value;
        }
    }

    private EnsRoom() { }
    internal EnsRoom(int id)
    {
        RoomId = id;
    }
    private static short t_connId;
    internal void Join(EnsConnection conn)
    {
        ClientConnections.Add(conn.ClientId,conn);
        conn.room = this;
        t_connId = conn.ClientId;
        Broadcast(conn.ClientId, Header.E, SendTo.Server, Delivery.Reliable, b =>
        {
            return ByteSerializer.Serialize(0x01,b.bytes,ref b.indexStart)
                && ShortSerializer.Serialize(t_connId, b.bytes, ref b.indexStart);
        });
        if (CurrentAuthorityAt == -1)
        {
            CurrentAuthorityAt = conn.ClientId;
            conn.Send(Header.A, SendTo.Server, SendTo.To(conn.ClientId), Delivery.Reliable, b =>
            {
                return BoolSerializer.Serialize(true, b.bytes, ref b.indexStart);
            });
        }
        else
        {
            conn.Send(Header.A, SendTo.Server, SendTo.To(conn.ClientId), Delivery.Reliable, b =>
            {
                return BoolSerializer.Serialize(false, b.bytes, ref b.indexStart);
            });
        }
    }
    internal void Exit(EnsConnection conn)
    {
        ClientConnections.Remove(conn.ClientId);
        conn.room = null; 
        conn.Send(Header.R, SendTo.Server, SendTo.To(conn.ClientId), Delivery.Reliable, null);
        if (conn.ClientId == CurrentAuthorityAt)
        {
            ShutDown();
        }
        else
        {
            Broadcast(conn.ClientId, Header.E, SendTo.Server, Delivery.Reliable, b =>
            {
                return ByteSerializer.Serialize(0x02, b.bytes, ref b.indexStart)
                    && ShortSerializer.Serialize(t_connId, b.bytes, ref b.indexStart);
            });
        }
    }
    internal void SetAuthority(short clientId)
    {
        if (!ClientConnections.ContainsKey(clientId)) return;
        if (ClientConnections.ContainsKey(CurrentAuthorityAt))
        {
            var conn = ClientConnections[CurrentAuthorityAt];
            conn.Send(Header.A, SendTo.Server, SendTo.To(conn.ClientId), Delivery.Reliable, b =>
            {
                return BoolSerializer.Serialize(false, b.bytes, ref b.indexStart);
            });
        }
        CurrentAuthorityAt= clientId;
        var c = ClientConnections[CurrentAuthorityAt];
        c.Send(Header.A, SendTo.Server, SendTo.To(c.ClientId), Delivery.Reliable, b =>
        {
            return BoolSerializer.Serialize(true, b.bytes, ref b.indexStart);
        });
    }

    internal void Broadcast(byte messageType, SendTo from, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        foreach (var i in ClientConnections.Values) i.Send(messageType,from,SendTo.To(i.ClientId),delivery,writer);
    }
    internal void Broadcast(int ignore, byte messageType, SendTo from, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        foreach (var i in ClientConnections.Values) 
            if (i.ClientId != ignore) 
                i.Send(messageType, from, SendTo.To(i.ClientId), delivery, writer);
    }
    internal void PTP(short id, byte messageType, SendTo from, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        if(ClientConnections.TryGetValue(id, out var conn))
        {
            conn.Send(messageType, from, SendTo.To(id), delivery, writer);
        }
    }

    internal void ShutDown()
    {
        Broadcast(Header.R, SendTo.Server, Delivery.Reliable, null);
        foreach(var i in ClientConnections)
        {
            i.Value.room = null;
        }
        EnsRoomManager.Instance.rooms.Remove(RoomId);
        Dispose();
    }



    protected override void ReleaseManagedMenory()
    {
        foreach(var i in ClientConnections.Values) i.Dispose();
        ClientConnections.Clear();
        base.ReleaseManagedMenory();
    }
    protected override void ReleaseUnmanagedMenory()
    {
        ClientConnections = null;
        base.ReleaseUnmanagedMenory();
    }

    public override string ToString()
    {
        string t = "[ " + RoomId.ToString() + " : ";
        bool first = true;
        foreach (var i in ClientConnections.Values)
        {
            if (!first) t += ",";
            t += i.ClientId;
            first = false;
        }
        t += "]";
        return t;
    }
}