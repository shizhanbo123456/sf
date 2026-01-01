using ProtocolWrapper;
using System;
using Utils;

//前5字节固定为分隔符，之后依次写入消息类型(1字节),消息目标(仅客户端向服务器方向如果有的话，2字节)，消息体，消息长度(2字节)
internal class SendBuffer:Disposable
{
    public const byte Separator = 0xFE;
    public const int MaxLength = 1400;
    public const int StartSeparatorLength = 5;//必须大于EndSeparatorLength
    public const int EndSeparatorLength = 3;

    private static ObjectPool<byte[]> BytePool = new ObjectPool<byte[]>(() => new byte[1420]);
    private ProtocolBase pb;
    internal SendBuffer(ProtocolBase pb)
    {
        bytes = BytePool.Get();
        this.pb = pb;
        indexStart = 0;
        while (indexStart < StartSeparatorLength)
            bytes[indexStart++] = Separator;
    }
    public byte[] bytes;
    public int indexStart=-1;


    internal void AddSeparator()
    {
        int indexEnd = indexStart + EndSeparatorLength;
        if (indexEnd > MaxLength) indexEnd = MaxLength;
        while (indexStart < indexEnd)
        {
            bytes[indexStart++] = Separator;
        }
    }
    internal void RequireLength(int length)
    {
        if (MaxLength - indexStart > length) return;
        pb.Send();
        indexStart = StartSeparatorLength;
    }
    internal void Flush()
    {
        pb.Send();
        indexStart = StartSeparatorLength;
    }
    internal void Clear()
    {
        indexStart = StartSeparatorLength;
    }

    protected override void ReleaseManagedMenory()
    {
        BytePool.Return(bytes);
        bytes = null;
    }
}