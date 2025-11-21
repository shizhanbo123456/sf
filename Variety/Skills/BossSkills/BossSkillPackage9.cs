using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss9
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
            var t = Target.GetNearestEnemy(99999,false);
            var d = Dt2Degree(t.transform.position - Target.transform.position);
            GetBullet(7).Init(new BulletAngleNonFacing(Target, 6, 5, d, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(7).Init(new BulletAngleNonFacing(Target, 6, 5, d+15, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            GetBullet(7).Init(new BulletAngleNonFacing(Target, 6, 5, d-15, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill1 : SkillBoss
    {
        private EffectCollection ec1;
        private EffectCollection ec2;
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 40f;
            restoreTime = 1;

            ec1 = new EffectCollection(t, (Effects.LifeSteal, 0.5f, 10));
            ec2 = new EffectCollection(t, (Effects.Silence, 0, 1));
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            foreach (var i in Ore.Ores.Values)
            {
                WarningCircle.Warn(i.transform.position, 3, 0.8f);
            }
            AddEvent(0.8f, (d) =>
            {
                foreach (var i in Ore.Ores.Values)
                {
                    GetBullet(11).Init(new BulletStatic(d.Target, 0.5f, 3, i.transform.position), new BulletDataStrike(d.Target, new Damage_Once(), 3f,ec:ec1)).Shoot();
                    GetBullet(6).Init(new BulletStatic(d.Target, 20, 3, i.transform.position), new BulletDataSlight(d.Target, new Damage_Time(0.5f), 0.2f, ec2)).Shoot();
                }
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 12f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(99999, false);
            var p = t.transform.position;
            WarningCircle.Warn(p, 3, 1);
            AddEvent(1f, (d) =>
            {
                for (int i = 0; i < 12; i++)
                {
                    float j = i * 30 * Mathf.Deg2Rad;
                    GetBullet(4).Init(new BulletProjectileAim(d.Target, 3, d.Target.transform.position, new Vector3(Mathf.Cos(j), Mathf.Sin(j))*8, p, 1.5f, 0.4f), new BulletDataCommon(d.Target, new Damage_Once(), 0.8f)).Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded =4f;
            cd = 25f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.transform.position;
            for(int i = -7; i <= 7; i += 2)
            {
                WarningRect.Warn(p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30), 5, 1);
            }
            AddEvent(1f, (d) =>
            {
                for(int i=-7;i<=7;i+=2)
                    GetBullet(12).Init(new BulletFromTo(d.Target,4f, p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30),1.5f), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
            });
            AddEvent(2f, (d) =>
            {
                for (int i = -8; i <= 8; i += 2)
                {
                    WarningRect.Warn(p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30), 5, 1);
                }
            });
            AddEvent(3f, (d) =>
            {
                for (int i = -8; i <= 8; i+=2)
                    GetBullet(12).Init(new BulletFromTo(d.Target, 4f, p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30), 1.5f), new BulletDataCommon(Target, new Damage_Once(), 1.2f)).Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        private EffectCollection ec;
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 20f;
            restoreTime = 1;

            ec = new EffectCollection(t, (Effects.AttackDecrease, 0.2f, 10), (Effects.BadLuck, 10, 10));
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(6,false).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 6, 1f);
            AddEvent(1f, (d) =>
            {
                GetBullet(6).Init(new BulletStaticScaleChange(d.Target,1f,6), new BulletDataSlight(d.Target, new Damage_Once(), 2.5f,ec:ec)).Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 45f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            
            WarningCircle.Warn(Target.transform.position, 30, 5f);
            AddEvent(5f, (d)=>
            {
                foreach (var i in Lantern.Lanterns.Values)
                {
                    var a = i.GetEnemyInRange(6, false);
                    foreach (var j in a)
                    {
                        j.ApplyEffect(new ArmorFortity(i, j, 95, 10));
                        j.ApplyEffect(new DamageBoost(i, j, 40, 10));
                    }
                }
                var c = d.Target.GetEnemyInRange(99999, false);
                foreach (var i in c) i.ApplyEffect(new Freeze(d.Target, i, 2));
                GetBullet(3).Init(new BulletStaticScaleChange(d.Target,1f,30), new BulletDataCommon(d.Target, new Damage_Once(), 32f)).Shoot();
            });
        }
    }
}