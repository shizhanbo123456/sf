using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Utils;

namespace ProtocolWrapper.Protocols.Udp
{
    internal abstract class WrapperUdp : ProtocolBase
    {
        protected UdpClient Client;
        protected IPEndPoint ep;
        public void Init(UdpClient client,IPEndPoint ep)
        {
            Client= client;
            this.ep= ep;
            base.Init(ep.Address.ToString(),ep.Port);
        }
        

        public override void RefreshSendBuffer()
        {
            if (!Initialized)
            {
                Debug.LogError("[W]未完成初始化");
                return;
            }
            if (Cancelled)
            {
                Debug.LogError("[W]已被取消");
                return;
            }
            if (SendBuffer == null || SendBuffer.indexStart <= StartSeparatorLength) return;

            try
            {
                Client.Send(SendBuffer.bytes, SendBuffer.indexStart,ep);
            }
            catch
            {

            }
            base.RefreshSendBuffer();
        }
        protected override void ReleaseManagedMenory()
        {
            Client.Dispose();
            Client = null;
            ep= null;
            base.ReleaseManagedMenory();
        }
    }
}