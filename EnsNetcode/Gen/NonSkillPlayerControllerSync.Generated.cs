using System;
using System.Collections.Generic;
using UnityEngine;

public partial class NonSkillPlayerControllerSync : EnsBehaviour, ITargetcontrollerInfo
{
    private static Dictionary<byte, Action<NonSkillPlayerControllerSync, byte[]>> FuncRecorder = new()
    {
        { 0, (p, b) => p.Invoke_SyncMotionRpc0(b) },
    };

    private static Dictionary<Action<string>, byte> map_string;

    public void CallFuncRpc(Action<string> func, SendTo sendto, Delivery delivery, string param1)
    {
        if (map_string == null) map_string = new()
        {
            { SyncMotionRpc, 0 },
        };

        if (!map_string.ContainsKey(func)) throw new Exception("目标函数未注册");

        EnsTemporaryBuffer.length=1;
        EnsTemporaryBuffer.bytes[0] = map_string[func];

        StringSerializer.Serialize(param1, EnsTemporaryBuffer.bytes, ref EnsTemporaryBuffer.length);
        Send(delivery, sendto);
    }

    private void Invoke_SyncMotionRpc0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        SyncMotionRpc(param1);
    }

    private static Segment RpcInvokeSegment;
    public override bool InvokeFunc(byte[] bytes,Segment s)
    {
        RpcInvokeSegment=s;
        byte funcId = bytes[s.StartIndex];
        if (FuncRecorder.TryGetValue(funcId, out var action))
        {
            action.Invoke(this, bytes);
            return true;
        }
        else
        {
            return base.InvokeFunc(bytes,s);
        }
    }
}
