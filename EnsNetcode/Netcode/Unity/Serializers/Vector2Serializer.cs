using System;
using UnityEngine;

public struct Vector2Serializer
{
    public static bool Serialize(Vector2 value, byte[] result, ref int indexStart)
    {
        // Vector2 = x(double 8字节) + y(double 8字节) → 共16字节
        if (result.Length - indexStart < 16) return false;

        DoubleSerializer.Serialize(value.x, result, ref indexStart);
        DoubleSerializer.Serialize(value.y, result, ref indexStart);
        return true;
    }

    public static Vector2 Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 16)
            throw new ArgumentException("反序列化Vector2失败：剩余数据不足16字节");

        float x = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float y = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        return new Vector2(x, y);
    }
}