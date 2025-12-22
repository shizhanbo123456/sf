using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillController:TargetSkillController
{
    private int skillIndex;
    private float interval;
    private float useSkillCD;

    public override void Init(Target data, Dictionary<string, string> param)
    {
        base.Init(data, param);
        if (param.ContainsKey("MInterval")) interval = float.Parse(param["MInterval"]);
        else interval = 5f;
    }
    protected override void Update()
    {
        base.Update();
        useSkillCD -= Time.deltaTime;
        if (useSkillCD < 0) UseSkill();
    }
    private void UseSkill()
    {
        if (Skills.Count == 0) return;
        if (skillIndex >= Skills.Count) skillIndex = 0;
        if (GetSkillByOwnedIndex(skillIndex).Detect(target))
        {
            var b = UseSkillByOwnedIndex(skillIndex);
            if (!b)
            {
                useSkillCD = 0.2f;
                Debug.Log("333");
            }
            else
            {
                useSkillCD = interval;
            }
        }
        else
        {
            Debug.Log("222");
        }
        skillIndex++;
    }
}