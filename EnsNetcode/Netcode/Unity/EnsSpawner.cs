using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 调用CreateServerRpc制造物体
/// 此类已经继承了NOMBehaviour来同步制造函数
/// </summary>
public sealed class EnsSpawner : MonoBehaviour
{
    [Space]
    public List<EnsBehaviourCollection> Prefabs = new List<EnsBehaviourCollection>();
    public Vector3 DefaultSpawnPosition = new Vector3(0, -10000, 0);

    private static short prefabid = 10000;

    private void Awake()
    {
        EnsInstance.EnsSpawner=this;

        for (int i = 0; i < Prefabs.Count; i++)
        {
            Prefabs[i].CollectionId = prefabid;
            prefabid++;
        }
    }
    private short GetBehaviourCount(short id)
    {
        foreach(var i in Prefabs)
        {
            if(i.CollectionId == id)
            {
                return (short)i.Count;
            }
        }
        throw new System.Exception("[E]无对应的物体");
    }

    private static short t_id;
    private static string t_param;
    private static short t_indexStart;
    public void CreateServerRpc(SendTo sendto, short id,string param)
    {
        t_id = id;
        t_param = param;
        t_indexStart=GetBehaviourCount(id);
        EnsInstance.Corr.Client.Send(Header.f, SendTo.To(EnsInstance.LocalClientId), sendto, Delivery.Reliable,Writer);
    }
    public void RespawnCheckServerRpc(SendTo sendto, EnsBehaviourCollection collection, string param)
    {
        t_id = collection.CollectionId;
        t_param = param;
        t_indexStart = collection.Behaviors[0].ObjectId;
        EnsInstance.Corr.Client.Send(Header.f, SendTo.To(EnsInstance.LocalClientId), sendto, Delivery.Reliable, Writer);
    }
    private static bool Writer(SendBuffer b)
    {
        return BoolSerializer.Serialize(false, b.bytes, ref b.indexStart)
            && ShortSerializer.Serialize(t_id, b.bytes, ref b.indexStart)
            && StringSerializer.Serialize(t_param, b.bytes, ref b.indexStart)
            && ShortSerializer.Serialize(t_indexStart, b.bytes, ref b.indexStart);
    }

    public void CreateLocal(short id,string param,short idStart)
    {
        Debug.Log(id + " " + param + " " + idStart);
        for (int i = 0; i < Prefabs.Count; i++)
        {
            if (Prefabs[i].CollectionId == id)
            {
                GameObject obj = Instantiate(Prefabs[i].gameObject, DefaultSpawnPosition,Quaternion.identity);
                var c = obj.GetComponent<EnsBehaviourCollection>();
                c.AllocateId(idStart);
                c.PreInit(param);

                return;
            }
        }
        Debug.LogError("[N]未找到目标物体");
    }
    public void RespawnCheckLocal(short id, string param, short idStart)
    {
        if (EnsNetworkObjectManager.HasObject(idStart)) return;
        for (int i = 0; i < Prefabs.Count; i++)
        {
            if (Prefabs[i].CollectionId == id)
            {
                GameObject obj = Instantiate(Prefabs[i].gameObject, DefaultSpawnPosition, Quaternion.identity);
                var c = obj.GetComponent<EnsBehaviourCollection>();
                c.AllocateId(idStart);
                c.PreRespawn(param);

                return;
            }
        }
        Debug.LogError("[N]未找到目标物体");
    }
}
