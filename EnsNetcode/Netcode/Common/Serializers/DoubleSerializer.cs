using System;

public struct DoubleSerializer
{
    public static bool Serialize(double value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 8) return false;
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes); // 转为大端序
        Buffer.BlockCopy(bytes, 0, result, indexStart, 8);
        indexStart += 8;
        return true;
    }

    public static double Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 8)
            throw new ArgumentException("反序列化double失败：剩余数据不足8字节");

        byte[] bytes = new byte[8];
        Buffer.BlockCopy(data, indexStart, bytes, 0, 8);
        Array.Reverse(bytes); // 还原小端序供解析
        indexStart += 8;
        return BitConverter.ToDouble(bytes, 0);
    }
}