using System;
using System.Collections.Generic;
using UnityEngine;
using AttributeSystem.Effect;
using System.Linq;

public partial class TargetDataSync : EnsBehaviour
{
    private static Dictionary<byte, Action<TargetDataSync, byte[]>> FuncRecorder = new()
    {
        { 0, (p, b) => p.Invoke_OnSyncHealthLocal0(b) },
        { 1, (p, b) => p.Invoke_Sgj0(b) },
        { 2, (p, b) => p.Invoke_Smz0(b) },
        { 3, (p, b) => p.Invoke_Sbj0(b) },
        { 4, (p, b) => p.Invoke_Sjs0(b) },
        { 5, (p, b) => p.Invoke_UseSkillLocal0(b) },
        { 6, (p, b) => p.Invoke_SyncEffectIconLocal0(b) },
        { 7, (p, b) => p.Invoke_DestroyLocal0(b) },
    };

    private static Dictionary<Action<string>, byte> map_string;

    public void CallFuncRpc(Action<string> func, SendTo sendto, Delivery delivery, string param1)
    {
        if (map_string == null) map_string = new()
        {
            { OnSyncHealthLocal, 0 },
            { Sgj, 1 },
            { Smz, 2 },
            { Sbj, 3 },
            { Sjs, 4 },
            { UseSkillLocal, 5 },
            { SyncEffectIconLocal, 6 },
        };

        if (!map_string.ContainsKey(func)) throw new Exception("目标函数未注册");

        EnsTemporaryBuffer.length=1;
        EnsTemporaryBuffer.bytes[0] = map_string[func];

        StringSerializer.Serialize(param1, EnsTemporaryBuffer.bytes, ref EnsTemporaryBuffer.length);
        Send(delivery, sendto);
    }

    private static Dictionary<Action, byte> map_void;

    public new void CallFuncRpc(Action func, SendTo sendto, Delivery delivery)
    {
        if (map_void == null) map_void = new()
        {
            { DestroyLocal, 7 },
        };

        if (!map_void.ContainsKey(func)) throw new Exception("目标函数未注册");

        EnsTemporaryBuffer.bytes[0] = map_void[func];

        Send(delivery, sendto);
    }

    private void Invoke_OnSyncHealthLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        OnSyncHealthLocal(param1);
    }

    private void Invoke_Sgj0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        Sgj(param1);
    }

    private void Invoke_Smz0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        Smz(param1);
    }

    private void Invoke_Sbj0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        Sbj(param1);
    }

    private void Invoke_Sjs0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        Sjs(param1);
    }

    private void Invoke_UseSkillLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        UseSkillLocal(param1);
    }

    private void Invoke_SyncEffectIconLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        SyncEffectIconLocal(param1);
    }

    private void Invoke_DestroyLocal0(byte[] bytes)
    {
        DestroyLocal();
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
