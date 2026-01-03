using System;

public struct UshortSerializer
{
    public static bool Serialize(ushort value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 2) return false;
        result[indexStart] = (byte)(value >> 8);
        result[indexStart + 1] = (byte)value;
        indexStart += 2;
        return true;
    }

    public static ushort Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 2)
        {
            Utils.Debug.LogError("反序列化失败：剩余数据字节数不足");
            return default;
        }

        ushort result = (ushort)((data[indexStart] << 8) | data[indexStart + 1]);
        indexStart += 2;
        if (indexStart > invalidIndex)
        {
            Utils.Debug.LogError("下标越界");
            return default;
        }
        return result;
    }
}