using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class TargetSkillController : EnsBehaviour
{
    public List<SkillBase> Skills = new List<SkillBase>();
    private HashSet<int>UseSkillCommandBuffer=new HashSet<int>();
    private float TimeNeeded = 0;

    private bool Initialized = false;

    public LockChain SkillLock;

    public virtual void Init(Target data)
    {
        Skills.Clear();
        var skills = Tool.VarietyManager.GetSkill(data);
        for (int i = 0; i < skills.Count; i++)
        {
            Skills.Add(skills[i]);
        }

        TimeNeeded = 0;

        SkillLock = data.SkillLock.GetChain();

        nomEnabled = data.UpdateLocally;

        Initialized = true;
    }
    public override void ManagedUpdate()
    {
        if (!Initialized) return;
        //此脚本如果不是本地会disable
        PreUpdate();

        foreach (var i in Skills) i.Update();

        if (TimeNeeded > 0)
        {
            TimeNeeded -= Time.deltaTime;
            SkillLock.Locked = true;
        }
        else SkillLock.Locked = false;

        if (SkillLock.LockedInHierechy)
        {
            UseSkillCommandBuffer.Clear();
            return;
        }

        foreach (var i in UseSkillCommandBuffer)
        {
            var s = Skills[i];
            if (!s.CanUse()) continue;
            TimeNeeded = s.TimeNeeded;
            CallFuncRpc(nameof(UseSkillRpc), SendTo.Everyone, i.ToString());
            break;
        }
        UseSkillCommandBuffer.Clear();
    }
    public bool UseSkillInstantly(int x)
    {
        if (SkillLock.LockedInHierechy) return false; 
        var s = Skills[x];
        if (!s.CanUse()) return false;
        TimeNeeded = s.TimeNeeded;
        CallFuncRpc(nameof(UseSkillRpc), SendTo.Everyone, x.ToString());
        return true;
    }
    public virtual void PreUpdate()
    {

    }
    public void UseSkill(int index)
    {
        if (!UseSkillCommandBuffer.Contains(index)) UseSkillCommandBuffer.Add(index);
    }
    private void UseSkillRpc(string index)
    {
        Skills[int.Parse(index)].UseSkill();
    }
    public void Interrupt()
    {
        foreach (var i in Skills) i.OnInterrupted();
        TimeNeeded = 0.5f;
    }
}
