using System;

public struct ShortSerializer
{
    public static bool Serialize(short value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 2) return false;
        result[indexStart] = (byte)(value >> 8);
        result[indexStart + 1] = (byte)value;
        indexStart += 2;
        return true;
    }

    public static short Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 2)
            throw new ArgumentException("反序列化short失败：剩余数据不足2字节");

        short result = (short)((data[indexStart] << 8) | data[indexStart + 1]);
        indexStart += 2;
        return result;
    }
}