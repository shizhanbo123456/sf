using SF.UI.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 设置为浮点表示冷却剩余比例，范围[0,1]
    /// </summary>
    public class SkillColumnCD : SkillColumnBase
    {
        /// <summary>
        /// 设置为浮点表示冷却剩余比例，范围[0,1]
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            PieShade.fillAmount = 1 - time;
        }
    }
}
public class SkillCDController : SkillBaseController
{
    private SkillColumnCD skill;
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
    public static SkillBaseController Create(int index,Target t, float cd,bool createUI)
    {
        var r = new SkillCDController();
        r.target = t;
        r.SkillIndex= index;
        if (createUI&&t && t is PlayerData p)
        {
            r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.CD) as SkillColumnCD;
            r.skill.SetSprite(Tool.SpriteManager.GetSprite(VarietyManager.GetSkill(index).sprite));
        }
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
    public override void OnDiscard()
    {
        if (skill != null) Tool.PageManager.PlayModePage.DestroySkillColumn(skill);
    }
}