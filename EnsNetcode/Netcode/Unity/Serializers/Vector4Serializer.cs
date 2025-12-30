using System;
using UnityEngine;

public struct Vector4Serializer
{
    public static bool Serialize(Vector4 value, byte[] result, ref int indexStart)
    {
        // Vector4 = x+y+z+w → 4*8=32字节
        if (result.Length - indexStart < 32) return false;

        DoubleSerializer.Serialize(value.x, result, ref indexStart);
        DoubleSerializer.Serialize(value.y, result, ref indexStart);
        DoubleSerializer.Serialize(value.z, result, ref indexStart);
        DoubleSerializer.Serialize(value.w, result, ref indexStart);
        return true;
    }

    public static Vector4 Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 32)
            throw new ArgumentException("反序列化Vector4失败：剩余数据不足32字节");

        float x = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float y = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float z = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float w = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        return new Vector4(x, y, z, w);
    }
}