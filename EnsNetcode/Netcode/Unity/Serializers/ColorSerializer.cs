using System;
using UnityEngine;

public struct ColorSerializer
{
    public static bool Serialize(Color value, byte[] result, ref int indexStart)
    {
        // Color = r+g+b+a → 4*8=32字节 (0~1浮点值)
        if (result.Length - indexStart < 32) return false;

        DoubleSerializer.Serialize(value.r, result, ref indexStart);
        DoubleSerializer.Serialize(value.g, result, ref indexStart);
        DoubleSerializer.Serialize(value.b, result, ref indexStart);
        DoubleSerializer.Serialize(value.a, result, ref indexStart);
        return true;
    }

    public static Color Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 32)
            throw new ArgumentException("反序列化Color失败：剩余数据不足32字节");

        float r = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float g = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float b = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float a = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return new Color(r, g, b, a);
    }
}