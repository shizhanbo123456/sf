using ProtocolWrapper;
using System;

/// <summary>
/// 实例化时启动客户端
/// </summary>
internal class EnsClient:SR
{
    protected KeyLibrary KeyLibrary;

    private ProtocolBase Client;

    protected bool _on;

    protected EnsClient(){ }
    internal EnsClient(string ip,int port)
    {
        KeyLibrary = new KeyLibrary();

        Client = Protocol.GetClient(ip,port);

        _on= true;
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        KeyLibrary.OnSend(messageType, sendFrom, target, delivery, writer);
    }
    internal override void Update()
    {
        if (hbRecvTime.Reached)
        {
            EnsInstance.Corr.ShutDown();
            return;
        }
        if (!Client.Initialized)
        {
            Utils.Debug.LogWarning("客户端初始化中");
            return;
        }
        KeyLibrary.Update();
        Client.Send();

        var q = Client.ReceiveBuffer;
        if (q == null) return;
        while(q.Read(out var data)&&_on)
        {
            try
            {
                if (data[1] == 'K' || data[1] == 'k')
                {
                    KeyLibrary.OnRecvData(data, out var skip, out data);
                    if (skip) continue;
                }
                MessageHandlerClient.Invoke(data);
            }
            catch
            {
            }
        }
    }
    internal override void ShutDown()
    {
        if (Client == null || Client.Cancelled) return;
        Client.SendData(Header.D);
        Client.Send();

        _on = false;
        Client.ShutDown();
        KeyLibrary.Clear();
    }
    internal override ProtocolBase GetProtocolBase()
    {
        return Client;
    }

    protected override void ReleaseManagedMenory()
    {
        Client?.Dispose();
        base.ReleaseManagedMenory();
    }
    protected override void ReleaseUnmanagedMenory()
    {
        Client = null;
        KeyLibrary= null;
        base.ReleaseUnmanagedMenory();
    }
}
