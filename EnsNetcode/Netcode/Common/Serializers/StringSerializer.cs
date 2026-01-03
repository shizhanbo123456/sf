using System;
using System.Text;

/// <summary>string 字符串 专属序列化器（UTF-8编码 | 大端序）</summary>
public struct StringSerializer
{
    /// <summary>
    /// 序列化字符串（UTF-8编码）
    /// </summary>
    /// <param name="value">待序列化的字符串（支持null/空字符串）</param>
    /// <param name="result">输出字节缓冲区</param>
    /// <param name="indexStart">起始索引（ref自动偏移）</param>
    /// <returns>缓冲区充足返回true，不足返回false</returns>
    public static bool Serialize(string value, byte[] result, ref int indexStart)
    {
        // 处理null：视为空字符串
        byte[] strBytes = string.IsNullOrEmpty(value)
            ? Array.Empty<byte>()
            : Encode(value);

        // 总所需字节数 = int长度标识(4字节) + UTF8真实字节数
        int totalNeedBytes = 4 + strBytes.Length;
        // 校验缓冲区剩余空间
        if (result.Length - indexStart < totalNeedBytes)
        {
            return false;
        }

        // 步骤1：先序列化【字符串UTF8字节长度】（复用你的IntSerializer，保证大端序统一）
        IntSerializer.Serialize(strBytes.Length, result, ref indexStart);
        // 步骤2：再序列化【字符串UTF8真实字节数据】
        if (strBytes.Length > 0)
        {
            Buffer.BlockCopy(strBytes, 0, result, indexStart, strBytes.Length);
            indexStart += strBytes.Length;
        }

        return true;
    }

    /// <summary>
    /// 反序列化字符串（UTF-8编码）
    /// </summary>
    /// <param name="data">输入字节数据</param>
    /// <param name="indexStart">起始索引（ref自动偏移）</param>
    /// <returns>反序列化后的字符串</returns>
    /// <exception cref="ArgumentException">数据不足/格式错误时抛异常</exception>
    public static string Deserialize(byte[] data, ref int indexStart, int invalidIndex)
    {
        // 先校验长度标识的基础4字节
        if (data.Length - indexStart < 4)
        {
            throw new ArgumentException("反序列化string失败：剩余数据不足，无法读取长度标识");
        }

        // 步骤1：先反序列化【字符串UTF8字节长度】
        int strByteLength = IntSerializer.Deserialize(data, ref indexStart,invalidIndex);

        // 校验真实字节数据的剩余空间
        if (strByteLength < 0 || data.Length - indexStart < strByteLength)
        {
            throw new ArgumentException($"反序列化string失败：长度标识({strByteLength}字节)超出剩余缓冲区大小");
        }

        // 步骤2：再反序列化【字符串UTF8真实字节数据】
        string result = string.Empty;
        if (strByteLength > 0)
        {
            result = Decode(data, indexStart, strByteLength);
            indexStart += strByteLength;
        }

        if (indexStart > invalidIndex)
            throw new ArgumentOutOfRangeException("index");
        return result;
    }
    public static byte[] Encode(string s)=>Encoding.UTF8.GetBytes(s);
    public static string Decode(byte[] b,int start,int length)=>Encoding.UTF8.GetString(b, start, length);
}