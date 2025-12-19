using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace ProtocolWrapper
{
    // 存储接收消息的结构体，包含内容和接收时间
    public struct ReceivedMessage
    {
        public string Content;
        public float ReceiveTime;
    }
    public static class Broadcast
    {
        public static int Port = 9900;
        public static float broadcastInterval = 1f;
        public static float CleanupExpiredMessagesInterval = 0.5f;
        public static float messageTimeout = 5f;

        private static ReachTime reachTime = new ReachTime(-1, ReachTime.InitTimeFlagType.ReachAt);
        private static ReachTime CleanExpiredTime= new ReachTime(-1, ReachTime.InitTimeFlagType.ReachAt);
        private static Dictionary<string, string> BroadcastContent = new Dictionary<string, string>();

        private static Dictionary<string, List<ReceivedMessage>> ReceiveContent = new Dictionary<string, List<ReceivedMessage>>();

        private static UdpClient senderClient;
        private static UdpClient receiverClient;
        private static IPEndPoint broadcastEndPoint;

        public static bool Sending => senderClient != null;
        public static bool Receiving=>receiverClient != null;

        public static bool StartBroadcast()
        {
            if (senderClient != null)
            {
                return true;
            }
            try
            {
                senderClient = new UdpClient();
                senderClient.EnableBroadcast = true;
                broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, Port);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static void BroadcastUpdate()
        {
            if (!reachTime.Reached) return;
            reachTime.ReachAfter(broadcastInterval);
            if (BroadcastContent.Count == 0) return;

            if (BroadcastContent.Count == 0) Debug.LogError("广播内容为空");
            var s=global::Format.DictionarySeparator+global::Format.DictionaryToString(BroadcastContent,wrapAll:false)+global::Format.DictionarySeparator;

            try
            {
                byte[] data = Format.GetBytes(s);
                senderClient.Send(data, data.Length, broadcastEndPoint);
            }
            catch
            {
                EndBroadcast();
                Debug.LogError("广播发送失败，已自动关闭广播");
            }
        }
        public static void EndBroadcast()
        {
            if (senderClient != null)
            {
                senderClient.Close();
                senderClient.Dispose();
                senderClient = null;
                broadcastEndPoint = null;
            }
        }

        public static void AddInfo(string header, string content)
        {
            if (BroadcastContent.ContainsKey(header))BroadcastContent[header] = content;
            else BroadcastContent.Add(header, content);
        }
        public static void RemoveInfo(string header)
        {
            if (BroadcastContent.ContainsKey(header))
                BroadcastContent.Remove(header);
        }
        public static void ClearSendContent()
        {
            BroadcastContent.Clear();
        }

        public static bool StartRecv()
        {
            if (receiverClient != null)
            {
                Debug.LogWarning("接收已经开启");
                return true;
            }
            try
            {
                receiverClient = new UdpClient();
                receiverClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                receiverClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
        private static void RecvUpdate()
        {
            try
            {
                while (receiverClient.Available > 0)
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedData = receiverClient.Receive(ref remoteEndPoint);
                    string message = Format.GetString(receivedData);

                    int msgstart = message.IndexOf(global::Format.DictionarySeparator);
                    int msgend = message.LastIndexOf(global::Format.DictionarySeparator);
                    if (msgstart == -1 || msgend == -1 || msgend <= msgstart) continue;

                    string msg = message.Substring(msgstart, msgend - msgstart + 1);
                    var s = global::Format.SplitWithBoundaries(msg, global::Format.DictionarySeparator,removeBoundary:false);
                    foreach (var part in s)
                    {
                        HandleReceivedMesg(part);
                    }
                }
            }
            catch
            {
                EndRecv();
                Debug.LogError("接收广播失败，已自动关闭广播");
            }
        }
        public static void EndRecv()
        {
            if (receiverClient != null)
            {
                receiverClient.Close();
                receiverClient.Dispose();
                receiverClient = null;
            }
        }

        private static void HandleReceivedMesg(string mesg)
        {
            var keyValue = global::Format.SplitWithBoundaries(mesg,global::Format.DictionaryPair);
            if (keyValue.Count != 2) return;
            string header = keyValue[0];
            string content = keyValue[1];

            // 存储消息（包含时间戳消息）
            var receivedMsg = new ReceivedMessage
            {
                Content = content,
                ReceiveTime = Time.time // 记录接收时的本地时间
            };

            if (ReceiveContent.ContainsKey(header))
            {
                bool exists = false;
                for (int i = 0; i < ReceiveContent[header].Count; i++)
                {
                    ReceivedMessage item = ReceiveContent[header][i];
                    if (item.Content == content)
                    {
                        exists = true;
                        ReceiveContent[header][i] = receivedMsg;
                        break;
                    }
                }
                if (!exists)
                {
                    ReceiveContent[header].Add(receivedMsg);
                }
            }
            else
            {
                ReceiveContent[header] = new List<ReceivedMessage> { receivedMsg };
            }
        }
        private static void CleanupExpiredMessages()
        {
            float currentTime = Time.time;
            List<string> headersToRemove = new List<string>();

            foreach (var kvp in ReceiveContent)
            {
                // 移除列表中超时的消息
                kvp.Value.RemoveAll(msg => currentTime - msg.ReceiveTime > messageTimeout);

                // 如果列表为空，标记整个键移除
                if (kvp.Value.Count == 0)
                {
                    headersToRemove.Add(kvp.Key);
                }
            }

            // 移除空列表的键
            foreach (var header in headersToRemove)
            {
                ReceiveContent.Remove(header);
            }
        }
        public static bool TryGetContents(string header, out List<ReceivedMessage> contents)
        {
            return ReceiveContent.TryGetValue(header, out contents);
        }
        public static void ClearRecvContent()
        {
            ReceiveContent.Clear();
        }

        public static void Update()
        {
            if (senderClient != null)
                BroadcastUpdate();

            if (receiverClient != null)
                RecvUpdate();

            if (CleanExpiredTime.Reached)
            {
                CleanExpiredTime.ReachAfter(CleanupExpiredMessagesInterval);
                CleanupExpiredMessages();
            }
        }
    }
}