using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class TargetDataSync : EnsBehaviour
{
    private Target target;
    public DedicateSyncAttributes DedicatedAttributes;
    public void Init(Target target, Dictionary<TargetParams, string> param)
    {
        this.target = target;
        nomEnabled = target.UpdateLocally;
        float healthRate = param.ContainsKey(TargetParams.HealthScale) ? float.Parse(param[TargetParams.HealthScale]) : 1;
        DedicatedAttributes = new DedicateSyncAttributes(target.Level,healthRate);
    }
    public override void ManagedUpdate()
    {
        if (HealthDirtyClearCD > 0) HealthDirtyClearCD -= Time.deltaTime;
        else if (HealthDirty)
        {
            HealthDirty = false;
            var sb = Tool.stringBuilder;
            sb.Append(DedicatedAttributes.Shengming.Value.Item1).Append('_').Append(DedicatedAttributes.Shengming.Value.Item2);
            CallFuncRpc(OnSyncHealthLocal, SendTo.ExcludeSender, Delivery.Reliable, sb.ToString());
            HealthDirtyClearCD = 0.15f;
        }
    }

    private float HealthDirtyClearCD;
    private bool HealthDirty;
    public void SyncShengming(int max,int present)
    {
        HealthDirty = true;
        DedicatedAttributes.Shengming.Value = (max,present);
    }
    [Rpc]
    private void OnSyncHealthLocal(string param)
    {
        var s = param.Split('_');
        DedicatedAttributes.Shengming.Value = (int.Parse(s[0]), int.Parse(s[1]));
    }
    public void SyncGongji(int value)
    {
        CallFuncRpc(Sgj,SendTo.ExcludeSender, Delivery.Reliable, value.ToString());
        DedicatedAttributes.Gongji = value;
    }
    [Rpc]
    private void Sgj(string data)
    {
        DedicatedAttributes.Gongji = int.Parse(data);
    }
    public void SyncMingzhong(int value)
    {
        CallFuncRpc(Smz,SendTo.ExcludeSender, Delivery.Reliable, value.ToString());
        DedicatedAttributes.Mingzhong = value;
    }
    [Rpc]
    private void Smz(string data)
    {
        DedicatedAttributes.Mingzhong = int.Parse(data);
    }
    public void SyncBaoji(int value)
    {
        CallFuncRpc(Sbj, SendTo.ExcludeSender, Delivery.Reliable, value.ToString());
        DedicatedAttributes.Baoji = value;
    }
    [Rpc]
    private void Sbj(string data)
    {
        DedicatedAttributes.Baoji = int.Parse(data);
    }
    public void SyncJiashang(int value)
    {
        CallFuncRpc(Sjs, SendTo.ExcludeSender, Delivery.Reliable, value.ToString());
        DedicatedAttributes.Jiashang = value;
    }
    [Rpc]
    private void Sjs(string data)
    {
        DedicatedAttributes.Jiashang = int.Parse(data);
    }
    public void UseSkillRpc(int index)
    {
        var sb = Tool.stringBuilder;
        sb.Append(index).Append('_').
            Append(target.FaceRight ? '1' : '0');
        CallFuncRpc(UseSkillLocal, SendTo.Everyone,Delivery.Unreliable, sb.ToString());
    }
    [Rpc]
    private void UseSkillLocal(string param)
    {
        string[] s = param.Split('_');
        int index = int.Parse(s[0]);
        bool faceright = s[1][0] == '1';
        VarietyManager.GetSkill(index).UseSkill(target, target.transform.position, faceright);
    }
    public void SyncEffectIconRpc(HashSet<(EffectType, int)> values)
    {
        if (values == null || values.Count == 0)
        {
            CallFuncRpc(SyncEffectIconLocal, SendTo.Everyone, Delivery.Unreliable,"null");
            return;
        }
        CallFuncRpc(SyncEffectIconLocal, SendTo.Everyone, Delivery.Unreliable,Format.ListToString(values, t=>((int)t.Item1).ToString(),'+'));
    }
    [Rpc]
    private void SyncEffectIconLocal(string data)
    {
        if (data == "null")
        {
            target.graphic.header.ShowEffects(new List<EffectType>());
            return;
        }
        var list = Format.StringToList(data, int.Parse, '+');
        target.graphic.header.ShowEffects(list.Select(i => (EffectType)i).ToList());
    }
    public void DestroyRpc()
    {
        CallFuncRpc(DestroyLocal, SendTo.Everyone, Delivery.Reliable);
    }
    [Rpc]
    private void DestroyLocal()
    {
        Destroy(gameObject);
    }
}