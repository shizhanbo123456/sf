using ProtocolWrapper;
using System;
using System.Collections.Generic;
using Utils;

public abstract class SR//具有信息收发功能
{
    internal float hbRecvTime = Time.time+EnsInstance.DisconnectThreshold;//new ReachTime(EnsInstance.DisconnectThreshold, ReachTime.InitTimeFlagType.ReachAfter);
    internal float hbSendTime = Time.time + EnsInstance.HeartbeatMsgInterval;//new ReachTime(EnsInstance.HeartbeatMsgInterval, ReachTime.InitTimeFlagType.ReachAfter);
    internal DeliverySource DeliverySource = DeliverySource.Get();

    protected static List<Segment> Parts = new List<Segment>();

    internal abstract void Send(byte messageType,SendTo sendFrom, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null);
    internal abstract void Update();
    internal virtual void ShutDown()
    {
        if (DeliverySource != null)
        {
            DeliverySource.Return(DeliverySource);
            DeliverySource = null;
        }
    }
    internal abstract ProtocolBase GetProtocolBase();


    internal static void Send(SendBuffer SendBuffer, byte messageType,SendTo sendFrom, SendTo target, byte delivery, Func<SendBuffer, bool> writer = null)
    {
        SendBuffer.RequireLength(376);
        int bytesStart = SendBuffer.indexStart;
        SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
        ShortSerializer.Serialize(sendFrom.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
        ShortSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
        ByteSerializer.Serialize(delivery, SendBuffer.bytes, ref SendBuffer.indexStart);
        if (writer != null)
        {
            var b = writer.Invoke(SendBuffer);
            if (!b)
            {
                //无法全部写入
                SendBuffer.Flush();
                bytesStart = SendBuffer.indexStart;
                SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
                ShortSerializer.Serialize(sendFrom.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
                ShortSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
                IntSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
                if (!writer.Invoke(SendBuffer))
                {
                    SendBuffer.Clear();
                    Debug.LogError("写入了超长数据");
                    return;
                }
            }
        }
        int length = SendBuffer.indexStart - bytesStart;
        ProtocolBase.LengthToBytes(length, out var a1, out var a2);
        SendBuffer.bytes[SendBuffer.indexStart++] = a1;
        SendBuffer.bytes[SendBuffer.indexStart++] = a2;
        SendBuffer.AddSeparator();
    }



    #region 分离出有效消息 -> ExtractData(byte[] data, List<Segment> parts)，传出的数据为消息头+消息体，不包含消息尾的长度

    private static List<Segment> segments = new List<Segment>();
    public static void ExtractData(byte[] data, List<Segment> parts)
    {
        segments.Clear();
        if (data == null || data.Length == 0 || parts == null) return;

        List<Segment> candidateSegments = SplitDataBySeparator(data);
        if (candidateSegments.Count == 0) return;

        int currentSegIndex = candidateSegments.Count - 1;
        while (currentSegIndex >= 0)
        {
            ProcessCandidateSegment(data, candidateSegments, ref currentSegIndex, parts);
        }
        parts.Reverse();
    }

    /// <summary>
    /// 按分隔符拆分，连续分隔符视为单一边界 | 修复：必处理最后一段数据
    /// 适配：消息首尾/消息间均有分隔符 → 拆分结果全为有效候选片段
    /// </summary>
    private static List<Segment> SplitDataBySeparator(byte[] data)
    {
        int start = 0;
        int len = data.Length;

        for (int i = 0; i < len; i++)
        {
            if (data[i] == SendBuffer.Separator)
            {
                if (i > start) segments.Add(new Segment(start, i - start));
                while (i < len && data[i] == SendBuffer.Separator) i++;
                start = i;
            }
        }
        //格式决定了无需处理最后一段，每个片段都被分隔符包裹了
        return segments;
    }

    private static void ProcessCandidateSegment(
        byte[] data,
        List<Segment> candidateSegments,
        ref int currentSegIndex,
        List<Segment> parts)
    {
        int checkStartIdx = candidateSegments[currentSegIndex].StartIndex;
        int checkTotalLength = candidateSegments[currentSegIndex].Length;
        int mergeSegIndex = currentSegIndex - 1;

        while (true)
        {
            // 最小长度阈值：固定头6字节 + 末尾长度段2字节 = 8字节
            if (checkTotalLength < 8) { currentSegIndex = mergeSegIndex; return; }

            // 提取末尾2字节Length段（已编码，无分隔符，无需校验）
            int lenRightIdx = checkStartIdx + checkTotalLength - 1;
            int lenLeftIdx = lenRightIdx - 1;
            int msgDeclaredLength = data[lenLeftIdx] * 200 + data[lenRightIdx];

            //严格遵循你的理解：核心长度校验逻辑
            if (checkTotalLength == msgDeclaredLength + 2)
            {
                parts.Add(new Segment(checkStartIdx, msgDeclaredLength));
                currentSegIndex = mergeSegIndex;
                return;
            }
            else if (checkTotalLength < msgDeclaredLength + 2)
            {
                // 长度不足 → 继续向前合并（被分隔符拆分）
                if (mergeSegIndex < 0) { currentSegIndex = mergeSegIndex; return; }
                checkStartIdx = candidateSegments[mergeSegIndex].StartIndex;
                checkTotalLength += candidateSegments[mergeSegIndex].Length;
                mergeSegIndex--;
            }
            else
            {
                // 长度超限 → 传输错误，放弃解析
                currentSegIndex = mergeSegIndex;
                return;
            }
        }
    }
    #endregion
}
public struct Segment
{
    public int StartIndex { get; } // ← 替代原MessagePart.start
    public int Length { get; }     // ← 替代原MessagePart.end（通过长度推导结束下标）

    public Segment(int startIndex, int length)
    {
        StartIndex = startIndex;
        Length = length;
    }
}