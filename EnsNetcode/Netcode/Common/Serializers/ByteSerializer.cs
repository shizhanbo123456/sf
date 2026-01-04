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

    public static byte Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 1)
        {
            Utils.Debug.LogError("反序列化失败：剩余数据字节数不足");
            throw new Exception();
        }

        byte result = data[indexStart];
        indexStart += 1;
        if (indexStart > invalidIndex)
        {
            Utils.Debug.LogError("下标越界");
            throw new Exception();
        }
        return result;
    }
}