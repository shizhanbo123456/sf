using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss4
{
    public class RepeatBoss4 : RepeatContent
    {
        public RepeatBoss4(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            int c = Ore.Ores.Values.Count;
            if (c == 0) return;
            target.ApplyEffect(new Speed(target.ObjectId, target, c * 0.7f,1));
            target.ApplyEffect(new DefenseBoost(target.ObjectId, target, c * 1.2f, 1));
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            var angle = Dt2Degree(t.transform.position - Target.transform.position);
            for(int i = 0; i < 20; i++)
            {
                AddEvent(0.1f * i, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.3f,liftstoiclevel:0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,5,10, angle+Mathf.Sin(d.index*0.5f)*45);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t=Target.GetNearestEnemy();
            t.ApplyMotion(new MotionDir(Vector2.up * 10, 0.7f, false, 1));
            AddEvent(0.7f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir((t.transform.position- d.Target.transform.position).normalized*30,1,true,1)); 
                var b = GetBullet(12);
                b.Init(2.5f,liftstoiclevel:2);
                BulletFollowSystem.RegistObject(b,3f,1f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
    public class Skill2 : SkillBoss
    {
        private EffectCollection ec1;
        private EffectCollection ec2;
        public Skill2() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            foreach(var i in Ore.Ores.Values)
            {
                var b = GetBullet(11);
                b.Init(3f, liftstoiclevel: 2, ec: new EffectCollection(Target, (EffectType.ArmorShatter, 55f, 10f)));
                BulletStaticSystem.RegistObject(b,3f,0.5f,i.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                b = GetBullet(6);
                b.Init(0.2f, liftstoiclevel: 0, ec: new EffectCollection(Target, (EffectType.Slowness, 2f, 1f)));
                BulletStaticSystem.RegistObject(b, 0,20f, i.transform.position);
                BulletDamageTimeSystem.Regist(b,0.5f);
                b.Shoot();
            }
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t=Target.GetNearestEnemy().transform.position;
            WarningRect.Warn(Target.transform.position, (t - Target.transform.position).normalized * 60 + Target.transform.position, 3, 1f);
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(12);
                b.Init(3.3f, liftstoiclevel: 2);
                BulletFromToSystem.RegistObject(b, 3f, 3,d.Target.transform.position, (t - d.Target.transform.position).normalized * 60 + d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 14f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy().transform.position;
            WarningCircle.Warn(t,2,0.5f);
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            AddEvent(1f, (d) =>
            {
                for(int i = 0; i < 3; i++)
                {
                    Vector3 v = new Vector3(Mathf.Cos(i * 120 * Mathf.Deg2Rad), Mathf.Sin(i * 120 * Mathf.Deg2Rad))*10;
                    var b = GetBullet(12);
                    b.Init(3.3f, liftstoiclevel: 2);
                    BulletFromToSystem.RegistObject(b,2,1,t+v,t-v);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            }); 
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(4);
            b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticSystem.RegistObject(b,8,4,Target.transform.position);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
        }
    }
}