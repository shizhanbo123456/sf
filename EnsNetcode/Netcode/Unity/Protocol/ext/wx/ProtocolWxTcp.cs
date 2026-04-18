using ProtocolWrapper.Protocols.WeChatTcp;
using System;
using System.Net;
using UnityEngine;
using WeChatWASM;

namespace Ens.DefaultProtocol
{
    public class ProtocolWxTcp : ProtocolBase
    {
        private void Start()
        {
            WX.InitSDK(_ => Debug.Log("初始化完成"));
        }
        public override ProtocolWrapper.ProtocolBase GetProtocolBase(string ip, int port)
        {
            ClientWeChatTcp client;
            try
            {
                client = new ClientWeChatTcp();
                client.Init(ip, port);
            }
            catch (Exception e)
            {
                Debug.LogError($"[WeChatUdp] 创建客户端失败：{e.ToString()}");
                return null;
            }
            return client;
        }
        public override ProtocolWrapper.ListenerBase GetListener(IPAddress ip, int port) =>
            throw new NotImplementedException("不支持Tcp服务器");
    }
}