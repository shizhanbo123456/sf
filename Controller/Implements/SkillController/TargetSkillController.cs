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

    public LockChain SkillLock;

    private bool Initialized = false;

    public virtual void Init(Target data, Dictionary<string, string> param)
    {
        target=data;

        if (param.ContainsKey("skillIndex"))
        {
            var skillIndex = Format.StringToList(param["skillIndex"], int.Parse);
            for (int i = 0; i < skillIndex.Count; i++)
            {
                CreateSkillColumn(data, skillIndex[i]);
            }
        }
        SkillLock = data.SkillLock.GetChain();

        TimeNeeded = 0;

        Initialized = true;
    }
    public virtual void CreateSkillColumn(Target data, int index)
    {
        Skills.Add(VarietyManager.GetSkill(index).CreateSkillController(data,index, false));
    }
    public virtual void DetroySkillColumnByIndex(int index)
    {
        if (index >= 0 && index < Skills.Count)
        {
            Skills[index].OnDiscard();
            Skills.RemoveAt(index);
        }
    }
    protected virtual void OnDestroy()
    {
        foreach (var i in Skills) i.OnDiscard();
        Skills.Clear();
    }
    protected virtual void Update()
    {
        if (!Initialized) return;
        PreUpdate();

        foreach (var i in Skills) i.Update();

        if (TimeNeeded > 0)
        {
            TimeNeeded -= Time.deltaTime;
            SkillLock.Locked = true;
        }
        else SkillLock.Locked = false;

        if (target.SkillLock.LockedInHierechy)
        {
            UseSkillCommandBuffer.Clear();
        }
        else
        {
            foreach (var i in UseSkillCommandBuffer)
            {
                if (UseSkillByOwnedIndex(i)) break;
            }
        }
        UseSkillCommandBuffer.Clear();
    }
    public virtual void PreUpdate()
    {

    }
    public bool UseSkillByOwnedIndex(int x)
    {
        if (SkillLock.LockedInHierechy) return false;
        if (x >= Skills.Count) return false;
        var s = Skills[x];
        if (!s.CanUse()) return false;
        var skill = VarietyManager.GetSkill(s.SkillIndex);
        TimeNeeded = skill.TimeNeeded;
        s.OnUse();
        target.UseSkillRpc(s.SkillIndex);
        return true;
    }
    public SkillBase GetSkillByOwnedIndex(int x) => (x<Skills.Count&&x>=0)?VarietyManager.GetSkill(Skills[x].SkillIndex):null;
    public void UseSkillBuffer(int index)
    {
        if (!UseSkillCommandBuffer.Contains(index)) UseSkillCommandBuffer.Add(index);
    }
}
