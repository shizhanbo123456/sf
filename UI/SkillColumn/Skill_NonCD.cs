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
    public class Skill_NonCD : SkillColumnBase
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
    private Skill_NonCD skill;
    private PlayerData Player;
    private int cost;
    public override void Update()
    {
        if (Player && skill != null)
        {
            if (Player.Mofa >= cost) skill.SetAvailableTime(1);
            else skill.SetAvailableTime(0);
        }
    }
    public override bool CanUse()
    {
        if (Player && Player.Mofa < cost) return false;
        return true;
    }
    public override void OnUse()
    {
        if (Player) Player.Mofa -= cost;
        base.OnUse();
    }
    public static SkillBaseController Create(int index,Target t, int cost)
    {
        var r = new SkillNonCDController();
        r.SkillIndex = index;
        if (t && t is PlayerData p)
        {
            r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.NonCD) as Skill_NonCD;
            r.Player = p;
        }
        r.cost = cost;
        return r;
    }
}