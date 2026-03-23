using LevelCreator.TargetTemplate;
using SF.UI.Skill;

public class SkillNonCDController : SkillBaseController
{
    private SkillColumnController skill;
    public override void Update()
    {
        skill.SetAvailableTime(1);
    }
    public override bool CanUse()
    {
        return base.CanUse();
    }
    public override void OnUse()
    {
        base.OnUse();
    }
    public override void OnDiscard()
    {
        if (skill != null) PlayModeController.Instance.DestroySkillColumn(skill);
    }
    public static SkillBaseController Create(int index,Target t,bool createUI)
    {
        var r = new SkillNonCDController();
        r.target = t;
        r.SkillIndex = index;
        if (createUI)
        {
            r.skill = PlayModeController.Instance.CreateSkillColumn(index);
            r.skill.SetLabelActive(false);
        }
        return r;
    }
}