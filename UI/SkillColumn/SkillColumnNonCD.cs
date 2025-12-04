using SF.UI.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 可用次数只可为0/1
    /// </summary>
    public class SkillColumnNonCD : SkillColumnBase
    {
        /// <summary>
        /// 只可为0/1
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            if (time > 0.5f) PieShade.fillAmount = 0;
            else PieShade.fillAmount = 1;
        }
    }
}
public class SkillNonCDController : SkillBaseController
{
    private SkillColumnNonCD skill;
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
    public static SkillBaseController Create(int index,Target t,bool createUI)
    {
        var r = new SkillNonCDController();
        r.target = t;
        r.SkillIndex = index;
        if (createUI&&t && t is PlayerData p)
        {
            r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.NonCD) as SkillColumnNonCD;
            r.skill.SetSprite(Tool.SpriteManager.GetSprite(VarietyManager.GetSkill(index).sprite));
        }
        return r;
    }
    public override void OnDiscard()
    {
        if (skill != null) Tool.PageManager.PlayModePage.DestroySkillColumn(skill);
    }
}