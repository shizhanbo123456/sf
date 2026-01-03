using System;
using UnityEngine;

public struct QuaternionSerializer
{
    public static bool Serialize(Quaternion value, byte[] result, ref int indexStart)
    {
        // Quaternion = x+y+z+w → 4*8=32字节
        if (result.Length - indexStart < 32) return false;

        DoubleSerializer.Serialize(value.x, result, ref indexStart);
        DoubleSerializer.Serialize(value.y, result, ref indexStart);
        DoubleSerializer.Serialize(value.z, result, ref indexStart);
        DoubleSerializer.Serialize(value.w, result, ref indexStart);
        return true;
    }

    public static Quaternion Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 32)
            throw new ArgumentException("反序列化Quaternion失败：剩余数据不足32字节");

        float x = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float y = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float z = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float w = (float)DoubleSerializer.Deserialize(data, ref indexStart, invalidIndex);
        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return new Quaternion(x, y, z, w);
    }
}