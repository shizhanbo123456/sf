using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetDataSync : EnsBehaviour
{
    private Target target;
    public DedicateSyncAttributes DedicatedAttributes;
    public void Init(Target target,Dictionary<string,string>param)
    {
        this.target = target;
        nomEnabled = target.UpdateLocally;
        float healthRate = param.ContainsKey("health") ? float.Parse(param["health"]) : 1;
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
            CallFuncRpc(nameof(OnSyncHealthLocal), SendTo.ExcludeSender, sb.ToString(),KeyLibrary.KeyFormatType.DisorderConfirm);
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
    private void OnSyncHealthLocal(string param)
    {
        var s = param.Split('_');
        DedicatedAttributes.Shengming.Value = (int.Parse(s[0]), int.Parse(s[1]));
    }
    public void SyncGongji(int value)
    {
        CallFuncRpc(nameof(Sgj),SendTo.ExcludeSender,value.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
        DedicatedAttributes.Gongji = value;
    }
    private void Sgj(string data)
    {
        DedicatedAttributes.Gongji = int.Parse(data);
    }
    public void SyncMingzhong(int value)
    {
        CallFuncRpc(nameof(Smz),SendTo.ExcludeSender,value.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
        DedicatedAttributes.Mingzhong = value;
    }
    private void Smz(string data)
    {
        DedicatedAttributes.Mingzhong = int.Parse(data);
    }
    public void SyncBaoji(int value)
    {
        CallFuncRpc(nameof(Sbj), SendTo.ExcludeSender, value.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
        DedicatedAttributes.Baoji = value;
    }
    private void Sbj(string data)
    {
        DedicatedAttributes.Baoji = int.Parse(data);
    }
    public void SyncJiashang(int value)
    {
        CallFuncRpc(nameof(Sjs), SendTo.ExcludeSender, value.ToString(), KeyLibrary.KeyFormatType.DisorderConfirm);
        DedicatedAttributes.Jiashang = value;
    }
    private void Sjs(string data)
    {
        DedicatedAttributes.Jiashang = int.Parse(data);
    }
    public void UseSkillRpc(int index)
    {
        var sb = Tool.stringBuilder;
        sb.Append(index).Append('_').
            Append(target.FaceRight ? '1' : '0');
        CallFuncRpc(nameof(UseSkillLocal), SendTo.Everyone, sb.ToString());
    }
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
            CallFuncRpc(nameof(SyncEffectIconLocal), SendTo.Everyone, "null");
            return;
        }
        CallFuncRpc(nameof(SyncEffectIconLocal), SendTo.Everyone, Format.ListToString(values, t=>((int)t.Item1).ToString(),'+'));
    }
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
}