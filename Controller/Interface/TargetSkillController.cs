using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSkillController : MonoBehaviour
{
    protected Target target;
    public List<SkillBaseController> Skills = new List<SkillBaseController>();
    private HashSet<int>UseSkillCommandBuffer=new HashSet<int>();
    private float TimeNeeded = 0;

    public LockChain SkillLock;

    private int repeatContentIndex;
    private float nextRepeatTime = -1;

    private bool Initialized = false;

    public virtual void Init(Target data, int[] skillIndex,int repeatContentIndex)
    {
        target=data;

        for (int i = 0; i < skillIndex.Length; i++)
        {
            CreateSkillColumn(data, skillIndex[i]);
        }
        SkillLock = data.SkillLock.GetChain();

        TimeNeeded = 0;

        this.repeatContentIndex= repeatContentIndex;

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

        UpdateRepeatContent();

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
                if (UseSkill(i)) break;
            }
        }
        UseSkillCommandBuffer.Clear();
    }
    public virtual void PreUpdate()
    {

    }
    public bool UseSkill(int x)
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
    public void UseSkillBuffer(int index)
    {
        if (!UseSkillCommandBuffer.Contains(index)) UseSkillCommandBuffer.Add(index);
    }
    private void UpdateRepeatContent()
    {
        if (repeatContentIndex < 0) return;
        if (Time.time > nextRepeatTime)
        {
            var c=VarietyManager.GetRepeatContent(repeatContentIndex);
            c.Repeat(target);
            nextRepeatTime = Time.time + c.dt;
        }
    }
}
