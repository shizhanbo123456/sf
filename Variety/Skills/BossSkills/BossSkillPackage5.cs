using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss5
{
    public class RepeatBoss5 : RepeatContent
    {
        public RepeatBoss5(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = Ore.Ores.Values.Count;
            if (c == 6) return;
            target.ApplyEffect(new Speed(target, target, (6-c) * 0.7f, 1));
            target.ApplyEffect(new DefenseBoost(target, target, (6-c) * 1.2f, 1));
        }
    }
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
            var t = Target.GetNearestEnemy();
            var angle = Dt2Degree(t.transform.position - Target.transform.position);

            var b = GetBullet(7);
            b.Init(0.3f,liftstoiclevel: 0);
            BulletAngleNonFacingSystem.RegistObject(b,0.3f,5f,10f,angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            for (int i = -4; i <=4; i++)
            {
                AddEvent(0.1f *Mathf.Abs(i),new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.3f,  liftstoiclevel: 0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,5f,10f,angle+d.index*5);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int i = 0; i < 5; i++)
            {
                float rad = (30 + i * 30)*Mathf.Deg2Rad;
                var b = GetBullet(7);
                b.Init(0.5f);
                BulletProectileSystem.RegistObject(b,1f,3f,Target.transform.position,new Vector3(Mathf.Cos(rad),Mathf.Sin(rad))*12);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1f;
            cd = 10f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 20, 1f, true, 2, true, true));
            AddEvent(1f, (d) =>
            {
                var p = d.Target.GetNearestEnemy();
                var angle= Dt2Degree(p.transform.position - Target.transform.position);
                for(int offset=-10;offset<=10;offset++)
                {
                    var b = GetBullet(7);
                    b.Init(0.5f, liftstoiclevel: 0);
                    BulletAngleSystem.RegistObject(b, 0.3f, 2f,15f, angle);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                };
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1.2f;
            cd = 20f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var list = Target.GetEnemyInRange();
            foreach (var i in list) WarningCircle.Warn(i.transform.position, 2, 1);
            AddEvent(1f, (d) =>
            {
                foreach (var i in list)
                {
                    for(int j=0;j<4;j++)
                    {
                        var b = GetBullet(12);
                        b.Init(0.3f);
                        BulletOrbitWorldSystem.RegistObject(b,0.8f,10f,4f,90,90*j,i.transform.position);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1.5f;
            cd = 3f;
            restoreTime = 1;
        }
        public override bool CanUse()
        {
            return base.CanUse()&&Target.GetEnemyInRange(8,true).Count>0;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy(80, true);
            if (t == null) return;
            WarningCircle.Warn(t.transform, 2.5f, 0.6f);
            Vector3 v=new Vector3();
            AddEvent(0.6f, (d) =>
            {
                v= t.transform.position;
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 2.5f, 0.4f, true, 1, true, true));
            });
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(7);
                b.Init(2.2f);
                BulletStaticSystem.RegistObject(b,2.5f,0.3f,v);
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
            TimeNeeded = 5;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 100; i++)
            {
                AddEvent(i * 0.05f, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.9f);
                    BulletAngleSystem.RegistObject(b, 0.3f, 2f, 20f, 7 * d.index);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}