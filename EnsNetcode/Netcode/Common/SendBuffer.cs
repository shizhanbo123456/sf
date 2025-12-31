using System;
using Utils;

//前5字节固定为分隔符，之后依次写入消息类型(1字节),消息目标(仅客户端向服务器方向如果有的话，2字节)，消息体，消息长度(2字节)
public class SendBuffer:Disposable
{
    private static ObjectPool<byte[]> BytePool = new ObjectPool<byte[]>(() => new byte[1420]);
    public SendBuffer()
    {
        bytes = BytePool.Get();
    }
    public byte[] bytes;
    public int indexStart=-1;


    protected override void ReleaseManagedMenory()
    {
        BytePool.Return(bytes);
        bytes = null;
    }
}