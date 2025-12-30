using System;
using UnityEngine;

public struct Vector3Serializer
{
    public static bool Serialize(Vector3 value, byte[] result, ref int indexStart)
    {
        // Vector3 = x(double 8) + y(double 8) + z(double 8) → 共24字节
        if (result.Length - indexStart < 24) return false;

        DoubleSerializer.Serialize(value.x, result, ref indexStart);
        DoubleSerializer.Serialize(value.y, result, ref indexStart);
        DoubleSerializer.Serialize(value.z, result, ref indexStart);
        return true;
    }

    public static Vector3 Deserialize(byte[] data, ref int indexStart)
    {
        if (data.Length - indexStart < 24)
            throw new ArgumentException("反序列化Vector3失败：剩余数据不足24字节");

        float x = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float y = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        float z = (float)DoubleSerializer.Deserialize(data, ref indexStart);
        return new Vector3(x, y, z);
    }
}