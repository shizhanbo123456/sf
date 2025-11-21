using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss7
{
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 4, 1);
            Target.ApplyMotion(new MotionDir(new Vector2(0, 20), 0.5f, true, 1, true, true));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.3f, true, 1, true, true));
            });
            AddEvent(0.8f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(Vector2.down*50,0.2f, true, 1, true, true));
            });
            AddEvent(0.8f, (d) =>
            {
                GetBullet(11).Init(new BulletStatic(d.Target,0.5f,4, d.Target.transform.position), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 0.5f, true, 1, true, true));
            for(int offset=-10;offset<=10;offset+=5)
            GetBullet(7).Init(new BulletAngle(Target, 3, 5, offset, 0.7f), new BulletDataCommon(Target, new Damage_Once(), 1.5f)).Shoot();
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.transform.position;
            Target.ApplyMotion(new MotionVelocityChange(Vector2.up*10,true,1,true,true));
            WarningCircle.Warn(p, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.right*5, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.left*5, 2, 0.5f);
            AddEvent(0.5f, (d) =>
            {
                GetBullet(11).Init(new BulletStatic(d.Target,0.6f,2,p), new BulletDataCommon(d.Target, new Damage_Once(), 1.4f)).Shoot();
                GetBullet(11).Init(new BulletStatic(d.Target,0.6f,2,p+Vector3.left*5), new BulletDataCommon(d.Target, new Damage_Once(), 1.4f)).Shoot();
                GetBullet(11).Init(new BulletStatic(d.Target,0.6f,2,p+Vector3.right*5), new BulletDataCommon(d.Target, new Damage_Once(), 1.4f)).Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 25f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(3, false).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(5).Init(new BulletFollow(Target,2f,3f), new BulletDataAttract(Target,0.2f,0.1f,10)).Shoot();
            Target.ApplyMotion(new MotionVelocityLerp(Vector2.up * 10, Vector2.up * 5, 2, true, 1, true, true));
            AddEvent(2, (d) =>
            {
                GetBullet(5).Init(new BulletDir(d.Target,2,10,new Vector2(2,-1),3), new BulletDataAttract(d.Target, 0.2f, 0.05f, 10)).Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(2,true, 1, true, true));
            GetBullet(5).Init(new BulletStaticScaleChange(Target,2,3,8), new BulletDataAttract(Target, 0.2f, 0.1f, 5)).Shoot();
            AddEvent(2, (d) =>
            {
                GetBullet(5).Init(new BulletDir(d.Target,2,8,Vector2.right,3), new BulletDataAttract(d.Target, 0.2f, 0.1f, 5)).Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        private EffectCollection ec;
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 60f;
            restoreTime = 1;

            ec = new EffectCollection(Target, (Effects.ArmorShatter, 20, 30));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionStatic(5, true, 1, true, true));
            GetBullet(5).Init(new BulletStaticScaleChange(Target, 5, 0,20), new BulletDataSlight(Target,new Damage_VFXOnly(),0f)).Shoot();
            AddEvent(5, (d) =>
            {
                GetBullet(5).Init(new BulletStaticScaleChange(d.Target, 1, 20), new BulletDataSlight(d.Target, new Damage_Once(), 2f,ec)).Shoot();
                var t=Target.GetPartnerInRange(999999, false);
                foreach(var i in t)
                {
                    i.ApplyEffect(new DamageBoost(d.Target, i, 30, 30));
                    i.ApplyEffect(new Luck(d.Target, i, 20, 30));
                }
            });
        }
    }
}