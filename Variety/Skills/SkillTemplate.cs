using SF.UI.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

//可通过继承BulletBase发射子弹，继承HitEventBase实现命中事件，设置EffectCollection实现效果
//技能中直接添加效果，使用 Local 版本
//Apply MotionBase实现移动控制，Apply TimeLineEvents实现延时调用
namespace Variety.Template
{
    public class SkillNonCD : SkillBase
    {
        protected Skill_NonCD skill;
        public SkillNonCD()
        {
            /*
            Name = "霰弹";
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            Tag = "平a";
            sprite = Tool.VarietyManager.GetSkillIcon(0, 1);
            TimeNeeded = 0.5f;
            cost = 30;
            */
        }
        public override SkillBaseContrller CreateSkillColumn(Target t)
        {
            return SkillNonCDController.Create(t, cost);
        }
        public sealed override bool CanUse(Target Target)
        {
            return true;
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            BulletSystemCommon.CurrentShooter = Target;
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            OnUse();
        }

        protected virtual void OnUse()
        {

        }
    }
    public class SkillCD : SkillBase
    {
        protected Skill_CD skill;
        protected float CD;

        public SkillCD()
        {
            /*
            Name = "霰弹";
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            Tag = "平a";
            sprite = Tool.VarietyManager.GetSkillIcon(0, 1);
            TimeNeeded = 0.5f;
            cost = 30;
            CD = 20f;
            storeTime = 1;//初始储存次数
            */
        }
        public override SkillBaseContrller CreateSkillColumn(Target t)
        {
            return SkillCDController.Create(t, cost, CD);
        }
        public sealed override bool CanUse(Target Target)
        {
            return true;
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            if (Target is PlayerData player) player.Mofa -= cost;
            BulletSystemCommon.CurrentShooter = Target;
            OnUse();
        }

        protected virtual void OnUse()
        {

        }
    }
    public class SkillStorable : SkillBase
    {
        protected Skill_Storable skill;
        protected float storeTime;
        protected float MaxstoreTime;
        protected float CD;

        public SkillStorable(Target t,Sprite s)
        {
            /*
            Name = "霰弹";
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            Tag = "平a";
            sprite = Tool.VarietyManager.GetSkillIcon(0, 1);
            TimeNeeded = 0.5f;
            cost = 30;
            MaxstoreTime = 3;
            storeTime = 3;//初始储存
            CD = 10f;
            */
        }
        public override SkillBaseContrller CreateSkillColumn(Target t)
        {
            return SkillStorableController.Create(t,cost,int.MaxValue,CD);
        }
        public sealed override bool CanUse(Target Target)
        {
            return true;
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            if (Target is PlayerData player) player.Mofa -= cost;
            storeTime -= 1;
            BulletSystemCommon.CurrentShooter = Target;
            OnUse();
        }
        protected virtual void OnUse()
        {
            
        }
    }
    public class SkillBoss : SkillBase
    {
        protected float cd;
        protected float restoreTime;
        public SkillBoss()
        {
            /*
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            TimeNeeded = 0.5f;
            cd=2f;
            restoreTime=1;
            */
        }
        public sealed override void UseSkill(Target Target, Vector3 pos, bool faceright)
        {
            restoreTime = 0;
            BulletSystemCommon.CurrentShooter = Target;
            OnUseSkill();
        }
        protected virtual void OnUseSkill()
        {
            
        }
        protected static float Dt2Degree(Vector3 dt)
        {
            return Mathf.Atan(dt.y / dt.x)*Mathf.Rad2Deg+(dt.x<0?180:0);
        }
        protected static Vector3 Angle2Vector(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}