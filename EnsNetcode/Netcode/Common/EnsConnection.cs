using ProtocolWrapper;
using System;
using System.Collections.Generic;

/// <summary>
/// 服务器使用，用于简化和客户端的通信
/// </summary>
public class EnsConnection:SR
{
    internal short ClientId;
    private KeyLibrary KeyLibrary;
    private ProtocolBase Connection;
    internal EnsRoom room;

    private Action<EnsConnection> OnShutDown;

    internal int delay = 20;//20ms

    protected bool _on;

    protected EnsConnection() { }
    internal EnsConnection(ProtocolBase _base,short index,Action<EnsConnection>onShutDown)
    {
        KeyLibrary = new KeyLibrary(Connection.SendBuffer,DeliverySource);

        Connection = _base;
        ClientId = index;
        OnShutDown = onShutDown;
        _on= true;

        Send(Header.C, SendTo.Server, SendTo.To(ClientId), Delivery.Reliable, ClientIdWriter);
    }
    private bool ClientIdWriter(SendBuffer buffer)
    {
        return IntSerializer.Serialize(ClientId, buffer.bytes,ref buffer.indexStart);
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        KeyLibrary.OnSend(messageType,sendFrom,target,delivery,writer);
    }
    internal override void Update()
    {
        KeyLibrary.Update();
        Connection.Send();

        var q=Connection.ReceiveBuffer;
        if (q == null) return;
        while (q.Read(out var data)&&_on)
        {
            ExtractData(data, Parts);
            foreach (var part in Parts) 
            {
                try
                {
                    KeyLibrary.OnRecvData(data, part, out bool skip);
                    if (skip) continue;
                    MessageHandlerServer.Invoke(this, data, part);
                }
                catch
                {
                }
            }
            Parts.Clear();
        }
    }
    internal override void ShutDown()
    {
        if (Connection==null||Connection.Cancelled) return;
        OnShutDown?.Invoke(this);
        Send(Header.D, SendTo.Server, SendTo.To(ClientId),Delivery.Unreliable);
        Connection.Send();

        _on = false;
        KeyLibrary.Clear();
        Connection.ShutDown();

        Dispose();
    }
    internal override ProtocolBase GetProtocolBase()
    {
        return Connection;
    }
    protected override void ReleaseManagedMenory()
    {
        Connection?.Dispose();
        base.ReleaseManagedMenory();
    }
    protected override void ReleaseUnmanagedMenory()
    {
        Connection = null;
        KeyLibrary = null;
        room = null;
        base.ReleaseUnmanagedMenory();
    }
}
