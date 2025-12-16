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
        public Vector2Int sprite;
        public string Name;
        public string Description;
        public string Tag;
        public float TimeNeeded=2;//释放所需的时间

        public virtual SkillBaseController CreateSkillController(Target t, int index, bool createUI) => throw new Exception("该类不可创建技能栏");
        public virtual bool CanUse(Target Target)
        {
            return true;
        }
        public virtual bool Detect(Target target)
        {
            return true;
        }
        public abstract void UseSkill(Target Target, Vector3 pos, bool faceright);
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