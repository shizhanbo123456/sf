using SF.UI.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;

//可通过继承BulletBase发射子弹，继承HitEventBase实现命中事件，设置EffectCollection实现效果
//技能中直接添加效果，使用 Local 版本
//Apply MotionBase实现移动控制，Apply TimeLineEvents实现延时调用
namespace Variety.Template
{
    public class SkillNonCD : SkillBase
    {
        protected Skill_NonCD skill;
        public SkillNonCD(Target t,Sprite s)
        {
            Target = t;
            sprite = s;
            /*
            Name = "霰弹";
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            Tag = "平a";
            sprite = Tool.VarietyManager.GetSkillIcon(0, 1);
            TimeNeeded = 0.5f;
            cost = 30;
            */

            if (t != null && t is PlayerData && (t as PlayerData).isLocalPlayer)
                skill = Tool.Instance.CreateSkillColumn<Skill_NonCD>(sprite);
        }
        public sealed override bool CanUse()
        {
            if (Target is PlayerData player && player.Mofa < cost) return false;
            return IfCanUse();
        }
        public sealed override void UseSkill()
        {
            if (Target is PlayerData player) player.Mofa -= cost;
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            OnUse();
        }
        public sealed override void Update()
        {
            if (skill != null)
            {
                if (Target is PlayerData player && player.Mofa >= cost)
                {
                    skill.SetAvailableTime(1);
                }
                else skill.SetAvailableTime(0);
            }
            OnUpdate();
        }

        protected virtual bool IfCanUse()
        {
            return true;
        }

        protected virtual void OnUse()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }
    public class SkillCD : SkillBase
    {
        protected Skill_CD skill;
        protected float storeTime;
        protected float CD;

        public SkillCD(Target t,Sprite s)
        {
            Target = t;
            sprite = s;

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

            if (t != null && t is PlayerData && (t as PlayerData).isLocalPlayer)
                skill = Tool.Instance.CreateSkillColumn<Skill_CD>(sprite);
        }
        public sealed override bool CanUse()
        {
            if (Target is PlayerData player && player.Mofa < cost) return false;
            return storeTime >= 0.99999f&&IfCanUse();
        }
        public sealed override void UseSkill()
        {
            if (Target is PlayerData player) player.Mofa -= cost;
            storeTime = 0;
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            OnUse();
        }
        public sealed override void Update()
        {
            storeTime += Time.deltaTime / CD;
            if (storeTime > 1) storeTime = 1;
            if (skill != null)
            {
                if (Target is PlayerData player && player.Mofa >= cost) skill.SetAvailableTime(storeTime);
                else skill.SetAvailableTime(0);
            }
            OnUpdate();
        }

        protected virtual bool IfCanUse()
        {
            return true;
        }

        protected virtual void OnUse()
        {

        }

        protected virtual void OnUpdate()
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
            Target = t;
            sprite = s;

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

            if (t != null && t is PlayerData && (t as PlayerData).isLocalPlayer)
                skill = Tool.Instance.CreateSkillColumn<Skill_Storable>(sprite);
        }
        public sealed override bool CanUse()
        {
            if (Target is PlayerData player && player.Mofa < cost) return false;
            return storeTime > 0.99999f&&IfCanUse();
        }
        public sealed override void UseSkill()
        {
            if (Target is PlayerData player) player.Mofa -= cost;
            storeTime -= 1;
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            OnUse();
        }
        public sealed override void Update()
        {
            storeTime += Time.deltaTime / CD;
            if (storeTime > MaxstoreTime) storeTime = MaxstoreTime;
            if (skill != null)
            {
                if (Target is PlayerData player && player.Mofa >= cost) skill.SetAvailableTime(storeTime);
                else skill.SetAvailableTime(0);
            }
            OnUpdate();
        }

        protected virtual bool IfCanUse()
        {
            return true;
        }

        protected virtual void OnUse()
        {
            
        }

        protected virtual void OnUpdate()
        {
            
        }
    }
    public class SkillBoss : SkillBase
    {
        protected Skill_NonCD skill;
        protected float cd;
        protected float restoreTime;
        public SkillBoss(Target t)
        {
            Target = t;
            TimeNeeded = -0.1f;
            /*
            Description = "向前发射三颗子弹，倍率0.6，耗魔10";
            TimeNeeded = 0.5f;
            cd=2f;
            restoreTime=1;
            */
        }
        public override bool CanUse()
        {
            return restoreTime > 0.999f;
        }
        public sealed override void UseSkill()
        {
            restoreTime = 0;
            OnUseSkill();
        }
        protected virtual void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
        public override void Update()
        {
            restoreTime += Time.deltaTime / cd;
            if (restoreTime > 1) restoreTime = 1;
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