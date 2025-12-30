using System;
using UnityEngine;

public struct Color32Serializer
{
    public static bool Serialize(Color32 value, byte[] result, ref int indexStart)
    {
        // Color32 = r+g+b+a → 4*1=4字节 (0~255字节值，性能最优)
        if (result.Length - indexStart < 4) return false;

        ByteSerializer.Serialize(value.r, result, ref indexStart);
        ByteSerializer.Serialize(value.g, result, ref indexStart);
        ByteSerializer.Serialize(value.b, result, ref indexStart);
        ByteSerializer.Serialize(value.a, result, ref indexStart);
        return true;
    }

    public static Color32 Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 4)
            throw new ArgumentException("反序列化Color32失败：剩余数据不足4字节");

        byte r = ByteSerializer.Deserialize(data, ref indexStart);
        byte g = ByteSerializer.Deserialize(data, ref indexStart);
        byte b = ByteSerializer.Deserialize(data, ref indexStart);
        byte a = ByteSerializer.Deserialize(data, ref indexStart);
        return new Color32(r, g, b, a);
    }
}