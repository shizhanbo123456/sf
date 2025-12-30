using System;

/// <summary>float 32位单精度浮点型 专属序列化器</summary>
public struct FloatSerializer
{
    public static bool Serialize(float value, byte[] result, ref int indexStart)
    {
        if (result.Length - indexStart < 4) return false;
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes); // 转为大端序
        Buffer.BlockCopy(bytes, 0, result, indexStart, 4);
        indexStart += 4;
        return true;
    }

    public static float Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 4)
            throw new ArgumentException("反序列化float失败：剩余数据不足4字节");

        byte[] bytes = new byte[4];
        Buffer.BlockCopy(data, indexStart, bytes, 0, 4);
        Array.Reverse(bytes); // 还原小端序供解析
        indexStart += 4;
        return BitConverter.ToSingle(bytes, 0);
    }
}