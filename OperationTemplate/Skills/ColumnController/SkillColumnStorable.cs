using LevelCreator.TargetTemplate;
using SF.UI.Skill;
using UnityEngine;

public class SkillStorableController : SkillBaseController
{
    private SkillColumnController skill;
    private int maxStoreTime;
    private float cd;
    private float storeTime;
    public override void Update()
    {
        storeTime += Time.deltaTime / cd;
        if (storeTime > maxStoreTime) storeTime = maxStoreTime;
        if (skill != null)
        {
            skill.SetAvailableTime(storeTime);
        }
    }
    public override bool CanUse()
    {
        return storeTime >= 0.999f && base.CanUse();
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
    public static SkillBaseController Create(int index, Target t, int maxStoreTime, float cd,bool createUI)
    {
        var r = new SkillStorableController();
        r.target = t;
        r.SkillIndex = index;
        if (createUI)
        {
            r.skill = PlayModeController.Instance.CreateSkillColumn(index);
            r.skill.SetLabelActive(true);
        }
        r.maxStoreTime = maxStoreTime;
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}