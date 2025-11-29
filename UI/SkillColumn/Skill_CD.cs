using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 设置为浮点表示冷却剩余比例，范围[0,1]
    /// </summary>
    public class Skill_CD : Skill_Base
    {
        /// <summary>
        /// 设置为浮点表示冷却剩余比例，范围[0,1]
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            PieShade.fillAmount = 1 - time;
        }
        
    }
    public class SkillCDController:SkillBaseContrller
    {
        private Skill_CD skill;
        private PlayerData Player;
        private int cost;
        private float cd;
        private float storeTime;
        public void Update()
        {
            storeTime += Time.deltaTime / cd;
            if (storeTime > 1) storeTime = 1;
            if (Player&&skill != null)
            {
                if (Player.Mofa >= cost) skill.SetAvailableTime(storeTime);
                else skill.SetAvailableTime(0);
            }
        }
        public override bool CanUse()
        {
            if (Player && Player.Mofa < cost) return false;
            return storeTime >= 0.999f;
        }
        public override void OnUse()
        {
            if (Player)Player.Mofa -= cost;
            storeTime -= 1f;
            base.OnUse();
        }
        public static SkillBaseContrller Create(Target t,int cost,float cd)
        {
            var r=new SkillCDController();
            if (t && t is PlayerData p)
            {
                r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.CD) as Skill_CD;
                r.Player = p;
            }
            r.cost=cost;
            r.cd=cd;
            r.storeTime=1;
            return r;
        }
    }
}