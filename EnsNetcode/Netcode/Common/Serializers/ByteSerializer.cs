using System;

public struct ByteSerializer
{
    public static bool Serialize(byte value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 1) return false;
        result[indexStart] = value;
        indexStart += 1;
        return true;
    }

    public static byte Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 1)
            throw new ArgumentException("反序列化byte失败：剩余数据不足1字节");

        byte result = data[indexStart];
        indexStart += 1;
        return result;
    }
}