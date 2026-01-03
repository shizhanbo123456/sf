using System;

public struct BoolSerializer
{
    public static bool Serialize(bool value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 1) return false;
        result[indexStart] = value ? (byte)1 : (byte)0;
        indexStart += 1;
        return true;
    }

    public static bool Deserialize(byte[] data, ref int indexStart,int invalidIndex)
    {
        if (data.Length - indexStart < 1)
            throw new ArgumentException("反序列化bool失败：剩余数据不足1字节");

        bool result = data[indexStart] != 0;
        indexStart += 1;
        if(indexStart>invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return result;
    }
}