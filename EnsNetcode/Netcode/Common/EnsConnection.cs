using ProtocolWrapper;
using System;

/// <summary>
/// 服务器使用，用于简化和客户端的通信
/// </summary>
public class EnsConnection:SR
{
    internal int ClientId;
    private KeyLibrary KeyLibrary;
    private ProtocolBase Connection;
    internal EnsRoom room;

    internal Action<EnsConnection> OnShutDown;

    internal int delay = 20;//20ms

    protected bool _on;

    protected EnsConnection() { }
    internal EnsConnection(ProtocolBase _base,int index)
    {
        KeyLibrary = new KeyLibrary();

        Connection = _base;
        ClientId = index;
        _on= true;

        SendData(Header.kC + ClientId);
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
            try
            {
                if (data[1] == 'K' || data[1]=='k')
                {
                    KeyLibrary.OnRecvData(data, out var skip, out data);
                    if (skip) continue;
                }
                MessageHandlerServer.Invoke(data, this);
            }
            catch
            {
            }
        }
    }
    internal override void ShutDown()
    {
        if (Connection==null||Connection.Cancelled) return;
        OnShutDown?.Invoke(this);
        Connection.SendData(Header.D);
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
