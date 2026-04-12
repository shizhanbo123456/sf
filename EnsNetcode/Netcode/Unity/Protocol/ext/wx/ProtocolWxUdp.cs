using ProtocolWrapper.Protocols.Tcp;
using ProtocolWrapper.Protocols.WeChatUdp;
using System;
using System.Net;
using UnityEngine;
using WeChatWASM;

namespace Ens.DefaultProtocol
{
    public class ProtocolWxUdp : ProtocolBase
    {
        private void Start()
        {
            WX.InitSDK(_=>Debug.Log("初始化完成"));
        }
        public override ProtocolWrapper.ProtocolBase GetProtocolBase(string ip, int port)
        {
            ClientWeChatUdp client;
            try
            {
                client = new ClientWeChatUdp();
                client.Init(ip, port);
            }
            catch (Exception e)
            {
                Debug.LogError($"[WeChatUdp] 创建客户端失败：{e.ToString()}");
                return null;
            }
            return client;
        }
        public override ProtocolWrapper.ListenerBase GetListener(IPAddress ip, int port)
        {
            ListenerTcp tcp;
            try
            {
                tcp = new ListenerTcp(ip, port);
            }
            catch (Exception e)
            {
                Utils.Debug.LogError(e.ToString());
                return null;
            }
            return tcp;
        }
    }
}