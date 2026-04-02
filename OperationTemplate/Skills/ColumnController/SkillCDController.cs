using LevelCreator.TargetTemplate;
using SF.UI.Skill;
using UnityEngine;

public class SkillCDController : SkillControllerBase
{
    private float cd;
    private float storeTime;
    public override void LoadSkillColumn(SkillColumn s)
    {
        base.LoadSkillColumn(s);
        s.SetLabelActive(false);
    }
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
    public static SkillControllerBase Create(short index,Target t, float cd)
    {
        var r = new SkillCDController();
        r.target = t;
        r.SkillIndex= index;
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}