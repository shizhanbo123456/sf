using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
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
            for(int offset = -15; offset <= 15; offset += 15)
            {
                var b = GetBullet(7);
                b.Init(0.5f, liftstoiclevel: 0);
                BulletAngleNonFacingSystem.RegistObject(b,0.3f,6f,5f,d+offset);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
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
                    var b = GetBullet(11);
                    b.Init(3,liftstoiclevel:2, ec:ec1);
                    BulletStaticSystem.RegistObject(b,3f,0.5f,i.transform.position);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                    b = GetBullet(6);
                    b.Init(0.2f,liftstoiclevel:0, ec: ec2);
                    BulletStaticSystem.RegistObject(b, 3f, 0.5f, i.transform.position);
                    BulletDamageTimeSystem.Regist(b,0.5f);
                    b.Shoot();
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
                    var b = GetBullet(4);
                    b.Init(0.8f);
                    BulletProectileAimSystem.RegistObject(b,0.4f,3f,d.Target.transform.position, new Vector3(Mathf.Cos(j), Mathf.Sin(j))*8, p, 1.5f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
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
                for(int i = -7; i <= 7; i += 2)
                {
                    var b = GetBullet(12);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b,1.5f,4f,p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
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
                {
                    var b = GetBullet(12);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b, 1.5f, 4f, p + new Vector3(i * 5, 10), p + new Vector3(i * 5, -30));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
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
                var b = GetBullet(6);
                b.Init(2.5f,liftstoiclevel:0,ec:ec);
                BulletStaticScaleChangeSystem.RegistObject(b,0f,6f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
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
                var b = GetBullet(3);
                b.Init(32f);
                BulletStaticScaleChangeSystem.RegistObject(b,0f,30f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}