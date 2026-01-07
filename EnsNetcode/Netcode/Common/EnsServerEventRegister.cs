using System.Collections.Generic;
using System.Text;
using Utils;
using request = Ens.Request;
public class EnsServerEventRegister
{
    public static void RegistDedicateServer()
    {
        Server_A();
        Server_H();
        Server_D();
        Server_F();
        Server_f();
        Server_Q();
        Server_Any();
        RegistServerRequests();
    }
    private static void RegistServerRequests()
    {
        EnsServerRequest.RegistRequest(new request.Server.CreateRoom());
        EnsServerRequest.RegistRequest(new request.Server.SetRule());
        EnsServerRequest.RegistRequest(new request.Server.JoinRoom());
        EnsServerRequest.RegistRequest(new request.Server.GetRule());
        EnsServerRequest.RegistRequest(new request.Server.ExitRoom());
        EnsServerRequest.RegistRequest(new request.Server.GetRoomList());
        EnsServerRequest.RegistRequest(new request.Server.SetInfo());
        EnsServerRequest.RegistRequest(new request.Server.GetInfo());
    }
    protected static void Server_Any()
    {
        MessageHandlerServer.RegistAny((conn,b,s) =>
        {
            conn.hbRecvTime=Time.time+EnsInstance.DisconnectThreshold;
        });
    }
    protected static void Server_H()
    {
        MessageHandlerServer.Regist(Header.H,(conn, b, s) =>
        {
            int index = s.StartIndex + 2;
            int invalidIndex = s.StartIndex + s.Length;
            var d = IntSerializer.Deserialize(b, ref index, invalidIndex);
            conn.delay = ((int)(Utils.Time.time * 1000) - d) / 2;
        });
    }
    protected static void Server_D()
    {
        MessageHandlerServer.Regist(Header.D,(conn, b, s) =>
        {
            conn.ShutDown();
        });
    }

    protected static void Server_A()
    {
        MessageHandlerServer.Regist(Header.A,(conn, b, s) =>
        {
            if (conn.room == null) return;
            int index = s.StartIndex + 2;
            int invalidIndex = s.StartIndex + s.Length;
            var d = ShortSerializer.Deserialize(b, ref index, invalidIndex);
            conn.room.SetAuthority(d);
        });
    }
    private static byte[] t_bytes;
    private static Segment t_segment;
    static bool BodyWriter(SendBuffer b)
    {
        if (b.bytes.Length - b.indexStart < t_segment.Length +2) return false;
        for (int i = t_segment.StartIndex + 6; i < t_segment.StartIndex + t_segment.Length; i++)
        {
            b.bytes[b.indexStart++] = t_bytes[i];
        }
        return true;
    }
    protected static void Server_F()
    {
        MessageHandlerServer.Regist(Header.F,(conn, b, s) =>
        {
            if (conn.room == null) return;
            byte messageType = b[s.StartIndex];
            SendTo to = SendTo.To(b[s.StartIndex + s.Length - 2], b[s.StartIndex + s.Length - 1]);
            Delivery delivery = DeliverySource.ByteToDelivery(b[s.StartIndex+1]);
            t_bytes = b;
            t_segment = new Segment(s.StartIndex,s.Length-2);
            if (to == SendTo.Everyone)
            {
                conn.room.Broadcast(messageType,delivery,BodyWriter);
            }
            else if (to == SendTo.ExcludeSender)
            {
                conn.room.Broadcast(conn.ClientId,messageType,delivery, BodyWriter);
            }
            else if (to == SendTo.RoomOwner)
            {
                conn.room.PTP(conn.room.CurrentAuthorityAt, messageType,delivery, BodyWriter);
            }
            else if (to == SendTo.Server) { }
            else
            {
                conn.room.PTP(to.Target, messageType,delivery, BodyWriter);
            }
        });
    }
    private static bool t_spawnMode;
    private static short t_spawnCollectionId;
    private static string t_spawnParam;
    private static short t_spawnIdStart;
    protected static void Server_f()
    {
        static bool BodyWriter(SendBuffer b)
        {
            return BoolSerializer.Serialize(t_spawnMode, b.bytes, ref b.indexStart)
                && ShortSerializer.Serialize(t_spawnCollectionId, b.bytes, ref b.indexStart)
                && StringSerializer.Serialize(t_spawnParam, b.bytes, ref b.indexStart)
                && ShortSerializer.Serialize(t_spawnIdStart, b.bytes, ref b.indexStart);
        }
        //物体Id同步
        MessageHandlerServer.Regist(Header.f,(conn, b, s) =>
        {
            if (conn.room == null) return;
            byte messageType = b[s.StartIndex];
            SendTo to = SendTo.To(b[s.StartIndex + s.Length - 2], b[s.StartIndex + s.Length - 1]);
            Delivery delivery = DeliverySource.ByteToDelivery(b[s.StartIndex + 1]);

            int indexStart = s.StartIndex+2;
            int invalidIndex = s.StartIndex + s.Length-2;
            t_spawnMode = BoolSerializer.Deserialize(b, ref indexStart, invalidIndex);
            t_spawnCollectionId=ShortSerializer.Deserialize(b, ref indexStart, invalidIndex);
            t_spawnParam= StringSerializer.Deserialize(b,ref indexStart, invalidIndex);
            t_spawnIdStart=ShortSerializer.Deserialize(b,ref indexStart, invalidIndex);
            if (!t_spawnMode)
            {
                //createdID+=spawnIdStart,spawnIdStart=createdID
                short t = conn.room.CreatedId;
                conn.room.CreatedId +=t_spawnIdStart;
                t_spawnIdStart = t;
            }
            if (to == SendTo.Everyone)
            {
                conn.room.Broadcast(messageType, delivery, BodyWriter);
            }
            else if (to == SendTo.ExcludeSender)
            {
                conn.room.Broadcast(conn.ClientId, messageType, delivery, BodyWriter);
            }
            else if (to == SendTo.RoomOwner)
            {
                conn.room.PTP(conn.room.CurrentAuthorityAt, messageType, delivery, BodyWriter);
            }
            else if (to == SendTo.Server) { }
            else
            {
                conn.room.PTP(to.Target, messageType,delivery, BodyWriter);
            }
        });
    }
    private static string t_header;
    private static string t_content;
    protected static void Server_Q()
    {
        MessageHandlerServer.Regist(Header.Q,(conn, b, s) =>
        {
            int index = s.StartIndex + 2;
            int invalidIndex = s.StartIndex + s.Length;
            t_header = StringSerializer.Deserialize(b, ref index, invalidIndex);
            t_content = StringSerializer.Deserialize(b, ref index, invalidIndex);
            string reply = EnsServerRequest.OnRecvRequest(t_header,t_content, conn);
            if (reply == string.Empty)
            {
                Utils.Debug.LogError($"对{t_header}:{t_content}的响应为空");
                return;
            }
            t_content = reply;
            conn.Send(Header.Q, Delivery.Reliable, buffer =>
            {
                return StringSerializer.Serialize(t_header, buffer.bytes, ref buffer.indexStart)
                    && StringSerializer.Serialize(t_content, buffer.bytes, ref buffer.indexStart);
            });
        });
    }
}