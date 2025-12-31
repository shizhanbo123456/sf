using ProtocolWrapper;
using System;
using System.Collections.Generic;
using Utils;

/// <summary>
/// Add OnReceive Update Clear
/// </summary>
public class KeyLibrary//处理的是未经NetcodeTool格式处理的信息，即"占用"中的信息
{
    public class SenderKey
    {
        private static Random Random = new Random();
        public enum KeyState
        {
            TobeConfirmed, ConfirmedExisting, End
        }

        public static int IndexRange = 100000000;
        public string Key;
        public int Index;
        public Delivery Type;
        public KeyState State = KeyState.TobeConfirmed;
        public float ToConfirmIntervalLeft;
        public float ConfirmedExistingSource;

        public SenderKey(string data, Delivery type)
        {
            ToConfirmIntervalLeft = -1f;
            ConfirmedExistingSource = EnsInstance.KeyExistTime + Time.time;
            Key = data;
            Index = Random.Next(IndexRange, IndexRange * 10 - 1);
            Type = type;
        }
    }
    public class ReceiverKey
    {
        public int Index;
        public float EndTime;
        public ReceiverKey(int index)
        {
            Index = index;
            EndTime = EnsInstance.RKeyExistTime + Time.time;
        }
    }
    private SR sr;

    private List<SenderKey> Keys = new List<SenderKey>();
    private List<SenderKey> TimeWiseKeys = new List<SenderKey>();
    private List<ReceiverKey> RecvKeys = new List<ReceiverKey>();

    private CircularQueue<string> Response = new CircularQueue<string>();//储存的格式：[K112233][F]....
    private CircularQueue<string> AddBuffer= new CircularQueue<string>();

    public KeyLibrary(SR sr)
    {
        this.sr= sr;
    }

    private static void UpdateEvent(SenderKey k, List<string>r)
    {
        switch (k.State)
        {
            case SenderKey.KeyState.TobeConfirmed:
                {
                    if (k.ConfirmedExistingSource>Time.time)
                    {
                        k.State = SenderKey.KeyState.End;
                    }

                    if (k.ToConfirmIntervalLeft > Time.time)
                    {
                        k.ToConfirmIntervalLeft=Time.time+EnsInstance.KeySendInterval;

                        string res;
                        if (k.Type == Delivery.Reliable) res = "[k" + k.Index + "]" + k.Key;
                        else res = "[K" + k.Index + "]" + k.Key;
                        r.Add(res);
                    }
                    break;
                }
            case SenderKey.KeyState.ConfirmedExisting:
                {
                    if (k.ConfirmedExistingSource>Time.time) k.State = SenderKey.KeyState.End;
                    break;
                }
        }
    }
    private static bool RecvEvent(SenderKey k)
    {
        bool skip = false;
        if (k.State == SenderKey.KeyState.ConfirmedExisting) skip = true;
        else if (k.State == SenderKey.KeyState.TobeConfirmed)
        {
            k.State = SenderKey.KeyState.ConfirmedExisting;
            skip = true;
        }
        return skip;
    }

    /// <summary>
    /// 程序内部传输，将需要进行检查的信息第一位'['改为'K'<br></br>
    /// 识别一个数据释放要进行检查的唯一依据就是data[0]=='K'<br></br>
    /// 传入KF]....，传入[K112233][F]....
    /// </summary>
    public void Add(byte key,byte messageType, SendTo target, Delivery delivery, Func<SendBuffer, bool> writer = null)
    {
        AddBuffer.Write(data);
    }
    public void Update()//传出[K112233][F]....
    {
        while(AddBuffer.Read(out var data))
        {
            data = Format(data,Delivery.Unreliable);
            if (data[0]=='k')Keys.Add(new SenderKey(data,Delivery.Reliable));
            else TimeWiseKeys.Add(new SenderKey(data, Delivery.OrderWise));
        }

        for (int i = RecvKeys.Count - 1; i >= 0; i--)
        {
            if (RecvKeys[i].EndTime>Time.time) RecvKeys.RemoveAt(i);
        }
        List<string> r = new List<string>();
        foreach (var k in Keys) UpdateEvent(k, r);
        if (TimeWiseKeys.Count > 0)
        {
            int index = 0;
            while (index < TimeWiseKeys.Count&&TimeWiseKeys[index].State != SenderKey.KeyState.TobeConfirmed) index++;

            for (int i = Math.Min(index,TimeWiseKeys.Count-1); i >= 0; i--) UpdateEvent(TimeWiseKeys[i], r);
        }
        for(int i = Keys.Count - 1; i >= 0; i--) if (Keys[i].State == SenderKey.KeyState.End) Keys.RemoveAt(i);
        for (int i = TimeWiseKeys.Count - 1; i >= 0; i--) if (TimeWiseKeys[i].State == SenderKey.KeyState.End) TimeWiseKeys.RemoveAt(i);

        while (Response.Read(out var d)) r.Add(d);
        return r;
    }
    public void Clear()
    {
        Keys.Clear();
        TimeWiseKeys.Clear();
    }
    /// <summary>
    /// 处理所有标记为K的信息<br></br>
    /// 本地无标记则发送回应确认收到<br></br>
    /// 传入[K112233][F]....，传出 KF]....
    /// </summary>
    public void OnRecvData(byte[] src,ProtocolBase.Segment segment, out bool skip)//传入[K112233][F].... [k112233][F]....
    {
        skip = false;
        //本地//////////////////////////////////////////////////////////////
        foreach (var k in Keys)
        {
            if (k.Index == index)
            {
                skip = RecvEvent(k);
                return;
            }
        }
        foreach (var k in TimeWiseKeys)
        {
            if (k.Index == index)
            {
                skip = RecvEvent(k);
                return;
            }
        }
        ////////////////////////////////////////////////////////////////////
        ///

        //非本地//////////////////////////////////////////////////////////////////
        Response.Write(data);
        foreach (var k in RecvKeys)
        {
            if (index == k.Index)
            {
                skip = true;
                break;
            }
        }
        if (!skip)
        {
            RecvKeys.Add(new ReceiverKey(index));
        }
    }
}