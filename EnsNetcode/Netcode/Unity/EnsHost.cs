using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于在服务器端也启动一个客户端<br></br>
/// 函数调用规则与ENCConnection一致
/// </summary>
internal class EnsHost : EnsConnection
{
    internal CircularQueue<byte[]> ReceivedData = new();
    private ENCLocalClient _client;
    private SendBuffer _buffer;

    internal static void Create(out EnsHost host,out ENCLocalClient client)
    {
        if (EnsInstance.Corr.Client != null)
        {
            Debug.LogError("[E]客户端已经启动");
            host = null;
            client = null;
            return;
        }
        client=new ENCLocalClient();
        EnsInstance.Corr.Client = client;
        host = new EnsHost(client);
        EnsInstance.Corr.Host = host;
    }
    internal EnsHost(ENCLocalClient client)
    {
        _client = client;
        _buffer=new SendBuffer(OnSend);
        ClientId = 0;
        EnsInstance.LocalClientId = ClientId;
        _on = true;
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        Send(_buffer, messageType, sendFrom, target, DeliverySource.Unreliable, writer);
    }
    private void OnSend(byte[] bytes,int length)
    {
        var b=BytesPool.GetBuffer(length);
        for(int i = 0; i < length; i++)
        {
            b[i]=bytes[i];
        }
        _client.ReceivedData.Write(b);
    }
    internal override void Update()
    {
        var q = ReceivedData;
        if (q == null) return;
        while (q.Read(out var data) && _on)
        {
            ExtractData(data, Parts);
            foreach (var part in Parts)
            {
                try
                {
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
        _client.ShutDown();
        _on = false;
    }
}