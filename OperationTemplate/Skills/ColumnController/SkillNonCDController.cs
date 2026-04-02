using LevelCreator.TargetTemplate;
using SF.UI.Skill;

public class SkillNonCDController : SkillControllerBase
{
    public override void LoadSkillColumn(SkillColumn s)
    {
        base.LoadSkillColumn(s);
        s.SetLabelActive(false);
    }
    public static SkillControllerBase Create(short index,Target t)
    {
        var r = new SkillNonCDController();
        r.target = t;
        r.SkillIndex = index;
        return r;
    }
}