using System;

/// <summary>
/// ENCLocalClient和ENCHost一起使用<br></br>
/// 在调用StartHost时由ENCHost创建
/// 函数调用规则与ENCClient一致
/// </summary>
internal class ENCLocalClient : EnsClient
{
    internal CircularQueue<byte[]> ReceivedData;
    private SendBuffer _buffer;
    public ENCLocalClient()
    {
        _on = true;
        ReceivedData=new CircularQueue<byte[]>();
        _buffer = new SendBuffer(OnSend);
        DeliverySource = DeliverySource.Get();
    }
    internal override void Send(byte messageType, SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        Send(_buffer, messageType, sendFrom, target, DeliverySource.GetIndex(delivery), writer);
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
        var buffer = ReceivedData;
        while (buffer.Read(out var data) && _on)
        {
            ExtractData(data, Parts);
            foreach (var part in Parts)
            {
                try
                {
                    MessageHandlerClient.Invoke(data, part);
                }
                catch(Exception e)
                {
                    Utils.Debug.ErrorCaught(e);
                }
            }
            Parts.Clear();
            BytesPool.ReturnBuffer(data);
        }
    }
    internal override void FlushSendBuffer()
    {
        _buffer.Flush();
    }
    internal override void ShutDown()
    {
        _on = false;
        ReceivedData = null;
        _buffer = null;
        DeliverySource.Return(DeliverySource);
        DeliverySource = null;
    }
}