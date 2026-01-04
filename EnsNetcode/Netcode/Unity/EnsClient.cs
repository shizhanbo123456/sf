using ProtocolWrapper;
using System;
using Utils;

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
        Protocol.OnClientInitialized += OnClientInitialized;

        Client = Protocol.GetClient(ip,port);

        _on = true;
    }
    private void OnClientInitialized()
    {
        Protocol.OnClientInitialized -= OnClientInitialized;
        KeyLibrary = new KeyLibrary(Client.SendBuffer, DeliverySource);
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        if (!Client.Initialized)
        {
            Debug.LogWarning("客户端初始化中");
            return;
        }
        KeyLibrary.OnSend(messageType, sendFrom, target, delivery, writer);
    }
    internal override void Update()
    {
        if (Time.time>hbRecvTime)
        {
            EnsInstance.Corr.ShutDown();
            return;
        }
        if (!Client.Initialized) return;
        var buffer = Client.ReceiveBuffer;
        while (buffer.Read(out var data)&&_on)
        {
            ExtractData(data);
            foreach (var part in segments)
            {
                try
                {
                    KeyLibrary.OnRecvData(data, part, out bool skip);
                    if (skip) continue;
                    MessageHandlerClient.Invoke(data, part);
                }
                catch (Exception e)
                {
                    Utils.Debug.ErrorCaught(e);
                }
                if (!_on) break;
            }
            segments.Clear();
        }
        if(_on)KeyLibrary.Update();
    }
    internal override void FlushSendBuffer()
    {
        if (!Client.Initialized) return;
        Client.SendBuffer.Flush();
    }
    internal override void ShutDown()
    {
        if (Client == null || Client.Cancelled) return;
        Send(Header.D, SendTo.To(EnsInstance.LocalClientId), SendTo.Server, Delivery.Unreliable);
        Client.SendBuffer.Flush();

        _on = false;
        base.ShutDown();
        KeyLibrary.Clear();
        Client.ShutDown();
        Client?.Dispose();
        Client = null;
        KeyLibrary = null;
    }
    internal override ProtocolBase GetProtocolBase()
    {
        return Client;
    }
}
