using UnityEngine;

public class MonsterSkillController:TargetSkillController
{
    private int skillIndex;
    private float interval;
    private float useSkillCD;

    public override void Init(Target data, int[] skillIndex, int repeatContentIndex)
    {
        base.Init(data, skillIndex, repeatContentIndex);
        interval = (data as Monster).StateInterval;
    }
    private void Update()
    {
        useSkillCD -= Time.deltaTime;
        if (useSkillCD < 0) UseSkill();
    }
    private void UseSkill()
    {
        if (Skills.Count == 0) return;
        if (skillIndex >= Skills.Count) skillIndex = 0;
        var b = UseSkill(skillIndex);
        if (!b)
        {
            useSkillCD = 0.3f;
        }
        else
        {
            useSkillCD = interval;
        }
        skillIndex++;
    }
}