using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class TargetSkillController : MonoBehaviour
{
    protected Target target;
    public List<SkillBaseController> Skills = new List<SkillBaseController>();
    private HashSet<int>UseSkillCommandBuffer=new HashSet<int>();
    private float TimeNeeded = 0;

    private bool Initialized = false;

    public LockChain SkillLock;

    public virtual void Init(Target data)
    {
        target=data;
        Skills.Clear();
        var skills = VarietyManager.GetSkillPackage(data);
        for (int i = 0; i < skills.Count; i++)
        {
            Skills.Add(skills[i].CreateSkillColumn(data));
        }

        TimeNeeded = 0;

        SkillLock = data.SkillLock.GetChain();

        Initialized = true;
    }
    private void Update()
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
            if(UseSkill(i))break;
        }
        UseSkillCommandBuffer.Clear();
    }
    public virtual void PreUpdate()
    {

    }
    public bool UseSkill(int x)
    {
        if (SkillLock.LockedInHierechy) return false; 
        var s = Skills[x];
        if (!s.CanUse()) return false;
        var skill = VarietyManager.GetSkill(s.SkillIndex);
        TimeNeeded = skill.TimeNeeded;
        s.OnUse();
        target.UseSkillRpc(s.SkillIndex);
        return true;
    }
    public void UseSkillBuffer(int index)
    {
        if (!UseSkillCommandBuffer.Contains(index)) UseSkillCommandBuffer.Add(index);
    }
}
