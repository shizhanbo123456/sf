using System;
using UnityEngine;

public struct Vector4Serializer
{
    public static bool Serialize(Vector4 value, byte[] result, ref int indexStart)
    {
        // Vector4 = x+y+z+w → 4*8=32字节
        if (result.Length - indexStart < 32) return false;

        FloatSerializer.Serialize(value.x, result, ref indexStart);
        FloatSerializer.Serialize(value.y, result, ref indexStart);
        FloatSerializer.Serialize(value.z, result, ref indexStart);
        FloatSerializer.Serialize(value.w, result, ref indexStart);
        return true;
    }

    public static Vector4 Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 32)
            throw new ArgumentException("反序列化Vector4失败：剩余数据不足32字节");

        float x = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float y = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float z = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float w = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return new Vector4(x, y, z, w);
    }
}