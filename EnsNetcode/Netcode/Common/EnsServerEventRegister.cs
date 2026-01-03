using System.Collections.Generic;
using System.Text;
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
            conn.hbRecvTime.ReachAfter(EnsInstance.DisconnectThreshold);
        });
    }
    protected static void Server_H()
    {
        MessageHandlerServer.Regist(Header.H,(conn, b, s) =>
        {
            int index = s.StartIndex + 6;
            int invalidIndex=s.StartIndex+ s.Length;
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
            int index = s.StartIndex + 6;
            int invalidIndex = s.StartIndex + s.Length;
            var d = ShortSerializer.Deserialize(b, ref index, invalidIndex);
            conn.room.SetAuthority(d);
        });
    }
    private static byte[] t_bytes;
    private static Segment t_segment;
    protected static void Server_F()
    {
        static bool BodyWriter(SendBuffer b)
        {
            if (b.bytes.Length - b.indexStart < t_segment.Length) return false;
            for (int i = t_segment.StartIndex; i < t_segment.StartIndex+t_segment.Length; i++)
            {
                b.bytes[b.indexStart++]=t_bytes[i];
            }
            return true;
        }
        MessageHandlerServer.Regist(Header.F,(conn, b, s) =>
        {
            if (conn.room == null) return;
            byte messageType = b[s.StartIndex];
            SendTo from = SendTo.To(b[s.StartIndex + 1], b[s.StartIndex + 2]);
            SendTo to = SendTo.To(b[s.StartIndex + 3], b[s.StartIndex + 4]);
            Delivery delivery = DeliverySource.ByteToDelivery(b[s.StartIndex+5]);
            t_bytes = b;
            t_segment = s;
            if (to == SendTo.Everyone)
            {
                conn.room.Broadcast(messageType, from, delivery,BodyWriter);
            }
            else if (to == SendTo.ExcludeSender)
            {
                conn.room.Broadcast(conn.ClientId,messageType, from, delivery, BodyWriter);
            }
            else if (to == SendTo.RoomOwner)
            {
                conn.room.PTP(conn.room.CurrentAuthorityAt, messageType, from, delivery, BodyWriter);
            }
            else if (to == SendTo.Server) { }
            else
            {
                conn.room.PTP(to.Target, messageType, from, delivery, BodyWriter);
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
            SendTo from = SendTo.To(b[s.StartIndex + 1], b[s.StartIndex + 2]);
            SendTo to = SendTo.To(b[s.StartIndex + 3], b[s.StartIndex + 4]);
            Delivery delivery = DeliverySource.ByteToDelivery(b[s.StartIndex + 5]);

            int indexStart = s.StartIndex+6;
            int invalidIndex = s.StartIndex + s.Length;
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
                conn.room.Broadcast(messageType, from, delivery, BodyWriter);
            }
            else if (to == SendTo.ExcludeSender)
            {
                conn.room.Broadcast(conn.ClientId, messageType, from, delivery, BodyWriter);
            }
            else if (to == SendTo.RoomOwner)
            {
                conn.room.PTP(conn.room.CurrentAuthorityAt, messageType, from, delivery, BodyWriter);
            }
            else if (to == SendTo.Server) { }
            else
            {
                conn.room.PTP(to.Target, messageType, from, delivery, BodyWriter);
            }
        });
    }
    private static string t_header;
    private static string t_content;
    protected static void Server_Q()
    {
        MessageHandlerServer.Regist(Header.Q,(conn, b, s) =>
        {
            int index = s.StartIndex + 6;
            int invalidIndex = s.StartIndex + s.Length;
            t_header = StringSerializer.Deserialize(b, ref index, invalidIndex);
            t_content = StringSerializer.Deserialize(b, ref index, invalidIndex);
            string reply = EnsServerRequest.OnRecvRequest(t_header,t_content, conn);
            if (reply == string.Empty) return;
            conn.Send(Header.Q, SendTo.Server, SendTo.To(conn.ClientId), Delivery.Reliable, b =>
            {
                return StringSerializer.Serialize(t_header, b.bytes, ref b.indexStart)
                    && StringSerializer.Serialize(t_content, b.bytes, ref b.indexStart);
            });
        });
    }
}