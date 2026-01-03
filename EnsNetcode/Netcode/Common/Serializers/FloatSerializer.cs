using System;

public struct FloatSerializer
{
    public static bool Serialize(float value, byte[] result, ref int indexStart)
    {
        if (result is null || indexStart < 0 || result.Length - indexStart < 4)
            return false;

        int num = BitConverter.SingleToInt32Bits(value);

        result[indexStart] = (byte)(num >> 24);
        result[indexStart + 1] = (byte)(num >> 16);
        result[indexStart + 2] = (byte)(num >> 8);
        result[indexStart + 3] = (byte)num;

        indexStart += 4;
        return true;
    }

    public static float Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data is null || indexStart < 0 || data.Length - indexStart < 4)
            throw new ArgumentException("反序列化float失败：剩余数据不足4字节或参数非法");

        int num = data[indexStart] << 24
                | data[indexStart + 1] << 16
                | data[indexStart + 2] << 8
                | data[indexStart + 3];

        indexStart += 4;

        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException(nameof(indexStart), "索引超出合法范围");

        return BitConverter.Int32BitsToSingle(num);
    }
}