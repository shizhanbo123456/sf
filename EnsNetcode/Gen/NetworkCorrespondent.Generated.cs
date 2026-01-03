using System;
using System.Collections.Generic;
using UnityEngine;
using Ens.Request.Client;
using System.Collections;

public partial class NetworkCorrespondent : EnsBehaviour
{
    private static Dictionary<byte, Action<NetworkCorrespondent, byte[]>> FuncRecorder = new()
    {
        { 0, (p, b) => p.Invoke_RecvData0(b) },
        { 1, (p, b) => p.Invoke_RecvAllData0(b) },
        { 2, (p, b) => p.Invoke_RecvNewData0(b) },
        { 3, (p, b) => p.Invoke_SetScoreboardActiveLocal0(b) },
        { 4, (p, b) => p.Invoke_SetScoreboardTextLocal0(b) },
        { 5, (p, b) => p.Invoke_CreateLevelLocal0(b) },
        { 6, (p, b) => p.Invoke_DestroyLevelLocal0(b) },
        { 7, (p, b) => p.Invoke_TargetKilledLocal0(b) },
        { 8, (p, b) => p.Invoke_ControllerStartFight0(b) },
        { 9, (p, b) => p.Invoke_RecvBackToPrepare0(b) },
    };

    private static Dictionary<Action<string>, byte> map_string;

    public void CallFuncRpc(Action<string> func, SendTo sendto, Delivery delivery, string param1)
    {
        if (map_string == null) map_string = new()
        {
            { RecvData, 0 },
            { RecvAllData, 1 },
            { RecvNewData, 2 },
            { SetScoreboardActiveLocal, 3 },
            { SetScoreboardTextLocal, 4 },
            { CreateLevelLocal, 5 },
            { TargetKilledLocal, 7 },
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
            { DestroyLevelLocal, 6 },
            { ControllerStartFight, 8 },
            { RecvBackToPrepare, 9 },
        };

        if (!map_void.ContainsKey(func)) throw new Exception("目标函数未注册");

        EnsTemporaryBuffer.bytes[0] = map_void[func];

        Send(delivery, sendto);
    }

    private void Invoke_RecvData0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        RecvData(param1);
    }

    private void Invoke_RecvAllData0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        RecvAllData(param1);
    }

    private void Invoke_RecvNewData0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        RecvNewData(param1);
    }

    private void Invoke_SetScoreboardActiveLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        SetScoreboardActiveLocal(param1);
    }

    private void Invoke_SetScoreboardTextLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        SetScoreboardTextLocal(param1);
    }

    private void Invoke_CreateLevelLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        CreateLevelLocal(param1);
    }

    private void Invoke_DestroyLevelLocal0(byte[] bytes)
    {
        DestroyLevelLocal();
    }

    private void Invoke_TargetKilledLocal0(byte[] bytes)
    {
        int indexStart = RpcInvokeSegment.StartIndex+1;
        int invalidIndex = RpcInvokeSegment.StartIndex+RpcInvokeSegment.Length;
        string param1 = StringSerializer.Deserialize(bytes, ref indexStart,invalidIndex);
        TargetKilledLocal(param1);
    }

    private void Invoke_ControllerStartFight0(byte[] bytes)
    {
        ControllerStartFight();
    }

    private void Invoke_RecvBackToPrepare0(byte[] bytes)
    {
        RecvBackToPrepare();
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
