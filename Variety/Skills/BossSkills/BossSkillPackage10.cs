using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss10
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
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            GetBullet(14).Init(new BulletProjectile(Target, 3,new Vector3(0,5)+front*5, 0.8f), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
            GetBullet(14).Init(new BulletProjectile(Target, 3,new Vector3(0,4)+front*4, 0.8f), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
            GetBullet(14).Init(new BulletProjectile(Target, 3,new Vector3(0,6)+front*6, 0.8f), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 22f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 3, 1.2f);
            AddEvent(1.2f, (d) =>
            {
                GetBullet(11).Init(new BulletStatic(d.Target,0.6f,3,d.Target.transform.position), new BulletDataCommon(d.Target, new Damage_Once(), 1.2f)).Shoot();
                Target.ApplyEffect(new AttackBoost(d.Target, Target, 0.3f, 10f));
            });
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
        public override bool CanUse()
        {
            return base.CanUse() && Target.GetEnemyInRange(12, true).Count > 0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up*12, 0.5f, true, 1, true, true));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.5f, true, 1, true,true));
                GetBullet(7).Init(new BulletAngle(d.Target, 1, 15, -15, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.5f)).Shoot();
                GetBullet(7).Init(new BulletAngle(d.Target, 1, 15, -30, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.5f)).Shoot();
                GetBullet(7).Init(new BulletAngle(d.Target, 1, 15, -45, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.5f)).Shoot();
                GetBullet(7).Init(new BulletAngle(d.Target, 1, 15, -60, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.5f)).Shoot();
                GetBullet(7).Init(new BulletAngle(d.Target, 1, 15, -75, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 0.5f)).Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(7).Init(new BulletOrbit(Target,10,6,120,0,0.7f), new BulletDataCommon(Target, new Damage_Once(), 0.4f)).Shoot();
            GetBullet(7).Init(new BulletOrbit(Target,10,6,120,180,0.7f), new BulletDataCommon(Target, new Damage_Once(), 0.4f)).Shoot();
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            WarningRect.Warn(Target.transform.position, Target.transform.position + front * 20, 4, 1.2f);
            AddEvent(1.2f, (d) =>
            {
                for(int j=-4;j<5;j+=2)
                    GetBullet(7).Init(new BulletProjectileAim(d.Target,2,d.Target.transform.position,new Vector3(0,j),d.Target.transform.position+front*10,1,0.7f), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 40f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 10, 1f, true, 1, true, true));
            AddEvent(1f, (d) =>
            {
                var t = Target.GetNearestEnemy(99999, false);
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 4, 0.25f, true, 1, true, true));
            });
            AddEvent(1.25f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.8f, true, 1, true, true));
                var t = d.Target.GetNearestEnemy(99999, false);
                GetBullet(7).Init(new BulletProjectileAim(d.Target,2,d.Target.transform.position,Vector3.down*10,t.transform.position,1f,3f), new BulletDataAttract(d.Target,0.2f,0.2f,15)).Shoot();
            });
        }
    }
}