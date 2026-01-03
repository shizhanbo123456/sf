using System;
using UnityEngine;

public struct Vector3Serializer
{
    public static bool Serialize(Vector3 value, byte[] result, ref int indexStart)
    {
        // Vector3 = x(double 8) + y(double 8) + z(double 8) → 共24字节
        if (result.Length - indexStart < 24) return false;

        FloatSerializer.Serialize(value.x, result, ref indexStart);
        FloatSerializer.Serialize(value.y, result, ref indexStart);
        FloatSerializer.Serialize(value.z, result, ref indexStart);
        return true;
    }

    public static Vector3 Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        if (data.Length - indexStart < 24)
            throw new ArgumentException("反序列化Vector3失败：剩余数据不足24字节");

        float x = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float y = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        float z = FloatSerializer.Deserialize(data, ref indexStart, invalidIndex);
        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return new Vector3(x, y, z);
    }
}