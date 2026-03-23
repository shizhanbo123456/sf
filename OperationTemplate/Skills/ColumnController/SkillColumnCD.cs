using LevelCreator.TargetTemplate;
using SF.UI.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCDController : SkillBaseController
{
    private SkillColumnController skill;
    private float cd;
    private float storeTime;
    public override void Update()
    {
        storeTime += Time.deltaTime / cd;
        if (storeTime > 1) storeTime = 1;
        if (skill != null)
        {
            skill.SetAvailableTime(storeTime);
        }
    }
    public override bool CanUse()
    {
        return storeTime >= 0.999f&&base.CanUse();
    }
    public override void OnUse()
    {
        storeTime -= 1f;
        base.OnUse();
    }
    public override void OnDiscard()
    {
        if (skill != null) PlayModeController.Instance.DestroySkillColumn(skill);
    }
    public static SkillBaseController Create(int index,Target t, float cd,bool createUI)
    {
        var r = new SkillCDController();
        r.target = t;
        r.SkillIndex= index;
        if (createUI)
        {
            r.skill = PlayModeController.Instance.CreateSkillColumn(index);
            r.skill.SetLabelActive(false);
        }
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}