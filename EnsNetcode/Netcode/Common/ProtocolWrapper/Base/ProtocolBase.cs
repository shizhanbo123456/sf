using System;
using System.Collections.Generic;
using Utils;

namespace ProtocolWrapper
{
    internal abstract class ProtocolBase : Disposable
    {
        public const int MaxLength = 1400;
        public const byte Separator = 0xFE;

        public const int StartSeparatorLength = 5;//必须大于EndSeparatorLength
        public const int EndSeparatorLength = 3;
        public string IP;
        public int Port;
        public SendBuffer SendBuffer;
        /// <summary>
        /// 取出后无需放回字节数组池，均为临时数组
        /// </summary>
        public CircularQueue<byte[]> ReceiveBuffer;

        public bool Initialized = false;
        public bool Cancelled = false;
        public bool On
        {
            get
            {
                return Initialized && !Cancelled;
            }
        }

        public int Id;

        /// <summary>
        /// Fill data only
        /// </summary>
        protected void Init(string ip, int port)
        {
            SendBuffer = new SendBuffer();
            InitSeparator(SendBuffer);
            ReceiveBuffer = new CircularQueue<byte[]>(4);
            IP = ip;
            Port = port;

            Id = Protocol.id++;
        }


        //直接对外暴露SendBuffer方便直接写入，省去复制
        public virtual void RefreshSendBuffer()
        {
            SendBuffer.indexStart = StartSeparatorLength;
        }

        public virtual void ShutDown()
        {
            Cancelled = true;
        }

        public static void Send(ProtocolBase pb,DeliverySource ds,byte messageType, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
        {
            SendBuffer SendBuffer = pb.SendBuffer;
            if (SendBuffer.indexStart >= 1300) pb.RefreshSendBuffer();
            int bytesStart = SendBuffer.indexStart;
            SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
            ShortSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
            ByteSerializer.Serialize(ds.GetIndex(delivery), SendBuffer.bytes, ref SendBuffer.indexStart);
            if (writer != null)
            {
                var b = writer.Invoke(SendBuffer);
                if (!b)
                {
                    //无法全部写入
                    pb.RefreshSendBuffer();
                    bytesStart = SendBuffer.indexStart;
                    SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
                    IntSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
                    writer.Invoke(SendBuffer);
                }
            }
            int length = SendBuffer.indexStart - bytesStart;
            LengthToBytes(length, out var a1, out var a2);
            SendBuffer.bytes[SendBuffer.indexStart++] = a1;
            SendBuffer.bytes[SendBuffer.indexStart++] = a2;
            AddSeparator(SendBuffer);
        }
        public static void Send(ProtocolBase pb, byte messageType, SendTo target, byte delivery, Func<SendBuffer, bool> writer = null)
        {
            SendBuffer SendBuffer = pb.SendBuffer;
            if (SendBuffer.indexStart >= 1300) pb.RefreshSendBuffer();
            int bytesStart = SendBuffer.indexStart;
            SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
            ShortSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
            ByteSerializer.Serialize(delivery, SendBuffer.bytes, ref SendBuffer.indexStart);
            if (writer != null)
            {
                var b = writer.Invoke(SendBuffer);
                if (!b)
                {
                    //无法全部写入
                    pb.RefreshSendBuffer();
                    bytesStart = SendBuffer.indexStart;
                    SendBuffer.bytes[SendBuffer.indexStart++] = messageType;
                    IntSerializer.Serialize(target.Target, SendBuffer.bytes, ref SendBuffer.indexStart);
                    writer.Invoke(SendBuffer);
                }
            }
            int length = SendBuffer.indexStart - bytesStart;
            LengthToBytes(length, out var a1, out var a2);
            SendBuffer.bytes[SendBuffer.indexStart++] = a1;
            SendBuffer.bytes[SendBuffer.indexStart++] = a2;
            AddSeparator(SendBuffer);
        }

        public static void InitSeparator(SendBuffer b)
        {
            b.indexStart = 0;
            while (b.indexStart < StartSeparatorLength)
                b.bytes[b.indexStart++] = Separator;
        }
        public static void AddSeparator(SendBuffer b)
        {
            int indexEnd = b.indexStart + EndSeparatorLength;
            if (indexEnd > MaxLength) indexEnd = MaxLength;
            while (b.indexStart < indexEnd)
            {
                b.bytes[b.indexStart++] = Separator;
            }
        }

        public static void BytesToLength(byte left, byte right, out int value)
        {
            value = left * 200 + right;
        }
        public static void LengthToBytes(int value, out byte left, out byte right)
        {
            left = (byte)(value / 200);
            right = (byte)(value % 200);
        }


        #region 分离出有效消息 -> ExtractData(byte[] data, List<Segment> parts

        /// <summary>
        /// 最终版解析方法：适配最新格式+复用Segment+新增messageFrom
        /// 输出：List<Segment> 中每个元素的StartIndex=body起始下标，Length=body字节长度
        /// 规则：无body时 → StartIndex > 0 && Length = 0（等价原end<start）
        /// </summary>
        /// <param name="data">原始字节数组</param>
        /// <param name="parts">输出参数：存储body片段的Segment集合</param>
        public static void ExtractData(byte[] data, List<Segment> parts)
        {
            // 1. 前置空值防护，杜绝运行时异常
            if (data == null || data.Length == 0 || parts == null) return;

            // 核心步骤1：按Separator预切割数组，生成候选片段列表（解决内部分隔符干扰）
            List<Segment> candidateSegments = SplitDataBySeparator(data);
            if (candidateSegments.Count == 0) return;

            // 核心步骤2：从后往前遍历候选片段，执行反向长度校验+拼接补偿
            int currentSegIndex = candidateSegments.Count - 1;
            while (currentSegIndex >= 0)
            {
                ProcessCandidateSegment(data, candidateSegments, ref currentSegIndex, parts);
            }
        }

        /// <summary>
        /// 按Separator拆分原始数组，返回候选片段（连续分隔符视为单个边界）
        /// 修复全局静态变量问题，纯局部变量，无数据污染
        /// </summary>
        private static List<Segment> SplitDataBySeparator(byte[] data)
        {
            List<Segment> segments = new List<Segment>();
            int start = 0;
            int len = data.Length;

            for (int i = 0; i < len; i++)
            {
                if (data[i] == Separator)
                {
                    if (i > start) segments.Add(new Segment(start, i - start));
                    // 跳过连续分隔符
                    while (i < len && data[i] == Separator) i++;
                    start = i;
                }
            }
            // 不处理最后一个不完整片段，等待下一轮拼接
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
                // ✅ 关键修改1：最小消息长度阈值 → 8字节
                // 计算依据：1+2+2+1 +2 =8 （固定头6字节 + 长度2字节）
                if (checkTotalLength < 8)
                {
                    currentSegIndex = mergeSegIndex;
                    return;
                }

                // 提取末尾2字节Length字段，校验合法性（禁止为Separator）
                int lenRightIdx = checkStartIdx + checkTotalLength - 1;
                int lenLeftIdx = lenRightIdx - 1;
                byte lenLeft = data[lenLeftIdx];
                byte lenRight = data[lenRightIdx];
                if (lenLeft == Separator || lenRight == Separator)
                {
                    currentSegIndex = mergeSegIndex;
                    return;
                }

                // 解析声明长度（包含：header+from+target+delivery+body）
                BytesToLength(lenLeft, lenRight, out int msgDeclaredLength);

                // 反向长度校验核心逻辑（不变）
                if (checkTotalLength == msgDeclaredLength)
                {
                    ParseValidBodyToSegment(data, checkStartIdx, msgDeclaredLength, parts);
                    currentSegIndex = mergeSegIndex;
                    return;
                }
                else if (checkTotalLength > msgDeclaredLength)
                {
                    currentSegIndex = mergeSegIndex;
                    return;
                }
                else
                {
                    if (mergeSegIndex < 0) { currentSegIndex = mergeSegIndex; return; }
                    checkStartIdx = candidateSegments[mergeSegIndex].StartIndex;
                    checkTotalLength += candidateSegments[mergeSegIndex].Length;
                    mergeSegIndex--;
                }
            }
        }

        /// <summary>
        /// ✅ 核心适配：解析合法消息，将BODY片段封装为Segment（完全复用，替代原MessagePart）
        /// 规则映射：Segment.StartIndex → BODY起始下标 | Segment.Length → BODY字节长度
        /// 无BODY时：Length=0（等价原end<start，调用方可通过Length==0判定）
        /// </summary>
        private static void ParseValidBodyToSegment(
            byte[] data,
            int msgStartIdx,
            int msgDeclaredLength,
            List<Segment> parts)
        {
            // 校验消息头合法性（禁止为Separator）
            byte msgHeader = data[msgStartIdx];
            if (msgHeader == Separator) return;

            // ✅ 关键修改2：BODY起始偏移 → +6
            // 计算依据：1(header)+2(from)+2(target)+1(delivery) =6 字节
            int bodyStartIdx = msgStartIdx + 6;
            // 计算BODY结束下标 & 实际长度
            int bodyEndIdx = msgStartIdx + msgDeclaredLength - 1;
            int bodyRealLength = bodyEndIdx - bodyStartIdx + 1;

            // ✅ 核心：复用Segment封装BODY片段，完美替代MessagePart
            Segment bodySegment;
            if (bodyRealLength > 0)
            {
                // 有BODY：正常赋值【起始下标+有效长度】
                bodySegment = new Segment(bodyStartIdx, bodyRealLength);
                parts.Add(bodySegment);
            }
            else
            {
                // 无BODY：按规则赋值【Length=0】（等价原end<start，调用方易判断）
                bodySegment = new Segment(bodyStartIdx, 0);
                parts.Add(bodySegment);
            }
        }

        /// <summary>
        /// 唯一复用的Segment结构体（既是切割候选片段，也是最终输出载体）
        /// ✅ 职责1：切割时 → 存储分隔符之间的候选片段信息
        /// ✅ 职责2：输出时 → 存储BODY片段的【起始下标】和【字节长度】
        /// </summary>
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
        #endregion

        protected override void ReleaseManagedMenory()
        {
            SendBuffer?.Dispose();
            SendBuffer = null;
            ReceiveBuffer = null;
            base.ReleaseManagedMenory();
        }
    }
}
