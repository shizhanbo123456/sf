using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NOMBehaviour提供了函数调用同步的功能
/// 调用CallFuncServerRpc即可实现同步调用<br></br>
/// 客户端主动调用的方法传入一个string类型的param，被调用时传入原来的string
/// 初始存在的物体需要enabled=true，销毁物体使用DestroyRpc或DestroyLocal
/// </summary>
public abstract class EnsBehaviour : MonoBehaviour
{
    // <0为玩家设置的初始化场景时制造的物体  >0为游戏过程中制造的物体的Id  =0为未分配的
    public int ObjectId=0;
    public bool nomEnabled = true;

    internal EnsBehaviourCollection collection;


    private static readonly Dictionary<Delivery, string> Key2Header = new Dictionary<Delivery, string>() 
    { 
        {Delivery.Unreliable,Header.F },
        {Delivery.Reliable,Header.kF },
        {Delivery.OrderWise,Header.KF }
    };
    public enum SendTo
    {
        Everyone=-1,
        ExcludeSender=-2,
        RoomOwner=-3
    }

    internal bool IdAutoAllocated=false;
    private bool startInvoked=false;

    protected void Start()
    {
        NOMStart();
        _Start();
    }
    internal void NOMStart()
    {
        if(startInvoked) return;
        startInvoked = true;
        if (ObjectId == 0)
        {
            ObjectId = EnsNetworkObjectManager.GetAutoAllocatedId();
            IdAutoAllocated = true;
        }
        else
        {
            if (!IdAutoAllocated)
            {
                int id = ObjectId % 100000000 + 2000000000;
                if (EnsNetworkObjectManager.ManualAssignedId.Contains(id))
                {
                    if(EnsNetworkObjectManager.HasObject(id))
                        Debug.LogError("手动分配的id发生冲突");
                }
                else
                {
                    EnsNetworkObjectManager.ManualAssignedId.Add(id);
                }
            }
        }
        EnsNetworkObjectManager.AddObject(this);
    }
    protected virtual void _Start()
    {
        
    }
    public void DestroyRpc(Delivery keyFormatType=Delivery.Reliable)
    {
        CallFuncRpc(nameof(DestroyLocal), SendTo.Everyone, keyFormatType);
    }
    public void DestroyLocal()
    {
        //foreach (var i in collection.Behaviors) i.NOMOnDestroy();//物体管理器移除物体+人为分配id移除物体
        if (collection != null) Destroy(collection.gameObject);
        else Destroy(gameObject);
    }
    /// <summary>
    /// 需要发送数据时，在此Update中使用，减少调用到传输的延迟<br></br>
    /// </summary>
    public virtual void ManagedUpdate()
    {

    }
    /// <summary>
    /// 需要发送数据时，在此FixedUpdate中使用，减少调用到传输的延迟<br></br>
    /// </summary>
    public virtual void FixedManagedUpdate()
    {

    }

    public void CallFuncRpc(string func, SendTo mode, Delivery type=Delivery.Unreliable )
    {
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.None)
        {
            if (mode != SendTo.ExcludeSender) StartCoroutine(func);
            return;
        }
        EnsInstance.Corr.Client.SendData(Key2Header[type] + ObjectId.ToString() + "#{" + func + "}#" + (int)mode);
    }
    public void CallFuncRpc(string func,List<int> targets, Delivery type = Delivery.Unreliable)
    {
        if (targets.Count == 0) return;
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.None)
        {
            if (targets.Contains(EnsInstance.LocalClientId)) StartCoroutine(func);
            return;
        }
        EnsInstance.Corr.Client.SendData(Key2Header[type] + ObjectId.ToString() + "#{" + func + "}#" + Format.ListToString(targets));
    }
    public void CallFuncRpc(string func, SendTo mode,string param, Delivery type = Delivery.Unreliable)
    {
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.None)
        {
            if (mode != SendTo.ExcludeSender) StartCoroutine(func,param);
            return;
        }
        EnsInstance.Corr.Client.SendData(Key2Header[type] + ObjectId.ToString() + "#{" + func + "}#" + (int)mode + "#{" + param+'}');
    }
    public void CallFuncRpc(string func, List<int> targets,string param, Delivery type = Delivery.Unreliable)
    {
        if (targets.Count == 0) return;
        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.None)
        {
            if (targets.Contains(EnsInstance.LocalClientId)) StartCoroutine(func, param);
            return;
        }
        EnsInstance.Corr.Client.SendData(Key2Header[type] + ObjectId.ToString() + "#{" + func + "}#" + Format.ListToString(targets) + "#{" + param+'}');
    }


    internal void CallFuncLocal(string func)
    {
        StartCoroutine(func);
    }
    internal void CallFuncLocal(string func, string param)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(func, param);
        }
    }
}
