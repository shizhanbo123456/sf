public abstract class SkillBaseController
{
    public int SkillIndex { get; protected set; }
    protected Target target;
    public virtual void Update()
    {

    }
    public virtual bool CanUse()
    {
        var skill = VarietyManager.GetSkill(SkillIndex);
        if (!skill.CanUse(target)) return false;
        return true;
    }
    public virtual void OnUse()
    {

    }
    public virtual void OnDiscard()
    {

    }
}