using ProtocolWrapper;
using System;
using Utils;

public abstract class SR : Disposable//具有信息收发功能
{
    internal ReachTime hbRecvTime = new ReachTime(EnsInstance.DisconnectThreshold, ReachTime.InitTimeFlagType.ReachAfter);
    internal ReachTime hbSendTime = new ReachTime(EnsInstance.HeartbeatMsgInterval, ReachTime.InitTimeFlagType.ReachAfter);
    internal DeliverySource DeliverySource = new DeliverySource();

    public abstract void Send(byte messageType, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null);
    internal abstract void Update();
    internal abstract void ShutDown();
    internal abstract ProtocolBase GetProtocolBase();

    protected override void ReleaseUnmanagedMenory()
    {
        hbRecvTime = null;
        hbSendTime = null;
    }
}