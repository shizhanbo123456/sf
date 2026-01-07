using ProtocolWrapper;
using System;
using System.Collections.Generic;
using Utils;

/// <summary>
/// Add OnReceive Update Clear
/// </summary>
internal class KeyLibrary
{
    //A发送，B收到消息后返回的校验消息中，Header不变，发送者为A，接收者为A，DeliveryType不变，body省略，length为6

    private class SenderKey
    {
        public enum KeyState
        {
            TobeConfirmed, ConfirmedExisting, End
        }
        public KeyState State;
        public float ToConfirmIntervalLeft;
        public float ConfirmedExistingSource;

        public byte messageType;
        public byte delivery;
        public Func<SendBuffer, bool> writer;

        public SenderKey(byte messageType,  byte delivery, Func<SendBuffer, bool> writer)
        {
            State = KeyState.TobeConfirmed;
            ToConfirmIntervalLeft = -1f;
            ConfirmedExistingSource = EnsInstance.KeyExistTime + Time.time;

            this.messageType = messageType;
            this.delivery = delivery;
            this.writer = writer;
        }
    }
    private class OrderedSenderKey:SenderKey
    {
        internal int index;
        public OrderedSenderKey(int index,byte messageType, byte delivery, Func<SendBuffer, bool> writer):
            base(messageType,delivery,writer)
        {
            this.index = index;
        }
    }
    private class ReceiverKey
    {
        public float EndTime;

        public byte[] bytes;
        public Segment segment;

        public ReceiverKey(byte[] src,Segment segment)
        {
            EndTime = EnsInstance.RKeyExistTime + Time.time;

            bytes = src;
            this.segment = segment;
        }
    }

    private Dictionary<byte,SenderKey> Keys = new();
    private Dictionary<byte, OrderedSenderKey> OrderedKeys = new();
    private Dictionary<byte, ReceiverKey> RecvKeys = new();
    private int OrderedIndexSource = 0;

    private readonly SendBuffer buffer;
    private readonly DeliverySource deliverySource;

    internal KeyLibrary(SendBuffer buffer,DeliverySource deliverySource)
    {
        this.buffer= buffer;
        this.deliverySource = deliverySource;
    }

    private static void UpdateEvent(SendBuffer buffer,SenderKey k)
    {
        switch (k.State)
        {
            case SenderKey.KeyState.TobeConfirmed:
                {
                    if (Time.time>k.ConfirmedExistingSource)
                    {
                        k.State = SenderKey.KeyState.End;
                        Utils.Debug.LogError($"信息未得到确认：type={k.messageType},delivery={k.delivery}");
                    }

                    if (Time.time>k.ToConfirmIntervalLeft)
                    {
                        k.ToConfirmIntervalLeft=Time.time+EnsInstance.KeySendInterval;

                        DataTransportBase.Send(buffer, k.messageType, k.delivery, k.writer);
                    }
                    break;
                }
            case SenderKey.KeyState.ConfirmedExisting:
                {
                    if (Time.time>k.ConfirmedExistingSource) k.State = SenderKey.KeyState.End;
                    break;
                }
        }
    }

    /// <summary>
    /// Add后自动发送，无需再发送
    /// </summary>
    internal void OnSend(byte messageType, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        var d=deliverySource.DeliveryToByte(delivery);
        DataTransportBase.Send(buffer, messageType, d, writer);
        if (delivery == Delivery.Unreliable) return;
        if(delivery== Delivery.Unreliable)
        {
            Keys.Add(d, new SenderKey(messageType, d, writer));
        }
        else
        {
            OrderedKeys.Add(d, new OrderedSenderKey(OrderedIndexSource++,messageType,d, writer));
        }
    }
    private static readonly HashSet<byte>ToRemove=new HashSet<byte>();
    internal void Update()
    {
        if (RecvKeys.Count > 0)
        {
            foreach (var kvp in RecvKeys) if (kvp.Value.EndTime > Time.time) ToRemove.Add(kvp.Key);
            foreach (var i in ToRemove) RecvKeys.Remove(i);
            ToRemove.Clear();
        }
        if (Keys.Count > 0)
        {
            foreach (var kvp in Keys)
            {
                UpdateEvent(buffer, kvp.Value);
                if (kvp.Value.State == SenderKey.KeyState.End) ToRemove.Add(kvp.Key);
            }
            foreach (var i in ToRemove) Keys.Remove(i);
            ToRemove.Clear();
        }
        if (OrderedKeys.Count > 0)
        {
            int index = int.MaxValue;
            foreach (var kvp in OrderedKeys)
            {
                if (kvp.Value.State == SenderKey.KeyState.ConfirmedExisting) UpdateEvent(buffer, kvp.Value);
                else if (kvp.Value.State == SenderKey.KeyState.TobeConfirmed) index = kvp.Value.index;
            }
            if(index!=int.MaxValue)
            foreach (var kvp in OrderedKeys)
            {
                if (kvp.Value.index==index) UpdateEvent(buffer,kvp.Value);
            }

            foreach (var kvp in OrderedKeys) if (kvp.Value.State == SenderKey.KeyState.End) ToRemove.Add(kvp.Key);
            foreach (var i in ToRemove) OrderedKeys.Remove(i);
            ToRemove.Clear();
            if (OrderedKeys.Count == 0) OrderedIndexSource = 0;
        }
    }
    internal void Clear()
    {
        Keys.Clear();
        OrderedKeys.Clear();
        RecvKeys.Clear();
    }
    /// <summary>
    /// 处理所有关键消息
    /// </summary>
    internal void OnRecvData(byte[] src,Segment segment, out bool skip)
    {
        var deliveryKey = src[segment.StartIndex + 1];
        if (deliveryKey == 0)
        {
            skip = false;
            return;
        }
        ///////////////////////////////////////////////////////////////A收到B对A的回应
        if (DeliverySource.IsResponse(deliveryKey))//发送者和接收者相同表示是对关键消息的回复
        {
            skip= true;
            deliveryKey=DeliverySource.ResponseToMessage(deliveryKey);
            if (Keys.ContainsKey(deliveryKey))
            {
                if (Keys[deliveryKey].State == SenderKey.KeyState.TobeConfirmed)
                {
                    Keys[deliveryKey].State = SenderKey.KeyState.ConfirmedExisting;
                }
            }
            else if (OrderedKeys.ContainsKey(deliveryKey))
            {
                if (OrderedKeys[deliveryKey].State == SenderKey.KeyState.TobeConfirmed)
                {
                    OrderedKeys[deliveryKey].State = SenderKey.KeyState.ConfirmedExisting;
                }
            }
        }
        //非本地//////////////////////////////////////////////////////////////////B收到A的关键消息
        else
        {
            if (!RecvKeys.ContainsKey(deliveryKey))
            {
                skip = false;
                RecvKeys.Add(deliveryKey,new ReceiverKey(src, segment));
            }
            else
            {
                skip = true;
            }
            DataTransportBase.Send(buffer, src[segment.StartIndex], DeliverySource.MessageToResponse(deliveryKey));
        }
    }
}