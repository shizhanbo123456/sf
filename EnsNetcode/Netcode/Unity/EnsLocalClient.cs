using System;
using UnityEngine;
/// <summary>
/// ENCLocalClient和ENCHost一起使用<br></br>
/// 在调用StartHost时由ENCHost创建
/// 函数调用规则与ENCClient一致
/// </summary>
internal class ENCLocalClient : EnsClient
{
    internal CircularQueue<byte[]> ReceivedData = new();
    private SendBuffer _buffer;
    public ENCLocalClient()
    {
        _on = true;
        _buffer = new SendBuffer(OnSend);
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        Send(_buffer, messageType, sendFrom, target, DeliverySource.Unreliable, writer);
    }
    private void OnSend(byte[] bytes, int length)
    {
        var b = BytesPool.GetBuffer(length);
        for (int i = 0; i < length; i++)
        {
            b[i] = bytes[i];
        }
        EnsInstance.Corr.Host.ReceivedData.Write(b);
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
                    MessageHandlerClient.Invoke(data, part);
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
        _on = false;
        ReceivedData = null;
    }
    protected override void ReleaseManagedMenory()
    {
        
    }
    protected override void ReleaseUnmanagedMenory()
    {
        ReceivedData = null;
    }
}