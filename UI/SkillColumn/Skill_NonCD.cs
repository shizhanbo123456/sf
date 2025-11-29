using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 可用次数只可为0/1
    /// </summary>
    public class Skill_NonCD : Skill_Base
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
    public class SkillNonCDController : SkillBaseContrller
    {
        private Skill_NonCD skill;
        private PlayerData Player;
        private int cost;
        public void Update()
        {
            if (Player && skill != null)
            {
                if (Player.Mofa >= cost) skill.SetAvailableTime(1);
                else skill.SetAvailableTime(0);
            }
        }
        public override bool CanUse()
        {
            if (Player&&Player.Mofa < cost) return false;
            return true;
        }
        public override void OnUse()
        {
            if (Player) Player.Mofa -= cost;
            base.OnUse();
        }
        public static SkillBaseContrller Create(Target t, int cost)
        {
            var r = new SkillNonCDController();
            if (t && t is PlayerData p)
            {
                r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.NonCD) as Skill_NonCD;
                r.Player = p;
            }
            r.cost = cost;
            return r;
        }
    }
}