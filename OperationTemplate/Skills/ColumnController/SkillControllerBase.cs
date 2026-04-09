using LevelCreator.Skills;
using LevelCreator.TargetTemplate;
using SF.UI.Skill;

public abstract class SkillControllerBase
{
    public ushort SkillIndex { get; protected set; }
    protected SkillColumn skill;
    protected Target target;
    public virtual void LoadSkillColumn(SkillColumn s)
    {
        skill = s;
    }
    public virtual void Update()
    {

    }
    public virtual bool CanUse()
    {
        return SkillExecuter.CanUse(SkillIndex,target);
    }
    public virtual void OnUse()
    {

    }
}