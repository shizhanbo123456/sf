using System;

/// <summary>sbyte 8位有符号字节型 专属序列化器</summary>
public struct SbyteSerializer
{
    public static bool Serialize(sbyte value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 1) return false;
        result[indexStart] = (byte)value;
        indexStart += 1;
        return true;
    }

    public static sbyte Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 1)
            throw new ArgumentException("反序列化sbyte失败：剩余数据不足1字节");

        sbyte result = (sbyte)data[indexStart];
        indexStart += 1;
        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return result;
    }
}