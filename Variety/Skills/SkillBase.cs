using SF.UI.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Base
{
    /// <summary>
    /// 可重写init,CanUse,UseSkill,Update<br></br>
    /// 可使用ApplyMotion ApplyMotionTo ApplyEvents(延时)
    /// </summary>
    public abstract class SkillBase
    {
        public Vector2 sprite;
        public string Name;
        public string Description;
        public string Tag;
        public float TimeNeeded;//释放所需的时间
        public int cost = 10;

        /// <summary>
        /// Init中需要对PlayerData是否为null进行判断(技能选择界面需要)<br></br>
        /// Init中需要对是否为Init中需要对PlayerData是否为null进行判断进行判断
        /// </summary>
        public SkillBase()
        {
            sprite=new Vector2();
            Name = "Test";
            Description = "Des";
            Tag = "近战";
            //sprite = Tool.VarietyManager.GetSkillIcon(0, 0);
            TimeNeeded = 2;
            cost = 30;
        }
        public virtual SkillBaseContrller CreateSkillColumn(Target t)
        {
            return null;
        }
        public virtual bool CanUse(Target Target)
        {
            return true;
        }
        public virtual void UseSkill(Target Target,Vector3 pos,bool faceright)
        {

        }
        protected void AddEvent(float delay,TimeLineData data,Action<TimeLineData>action)
        {
            BulletSystemCommon.CurrentShooter.TimeLineWork.AddEvent(delay, data, action);
        }
        protected void AddEvent(float delay, Action<TimeLineData> action)
        {
            BulletSystemCommon.CurrentShooter.TimeLineWork.AddEvent(delay,new TimeLineData(BulletSystemCommon.CurrentShooter), action);
        }
        /// <summary>
        /// 0-2:不可变色，3:魔法核,4:能量球,5:能量球(吸收),6:能量球(放射)<br></br>
        /// 7:距离,8:光点,9:魔法阵,10:雪球,11:爆炸,12:火球<br></br>
        /// 13-15:雾,16:烟火
        /// </summary>
        protected static Bullet GetBullet(int index)
        {
            return Tool.BulletManager.GetBullet(index);
        }
    }
}