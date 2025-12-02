using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss8
{
    public class RepeatBoss8 : RepeatContent
    {
        public RepeatBoss8(Target t) : base(t)
        {
            dt = 1f;
        }
        protected override void Repeat()
        {
            bool f = false;
            foreach(var i in Lantern.Lanterns.Values) 
                if (!i.Alive)
                {
                    f=true; 
                    break;
                }
            if (f)
            {
                foreach(var i in target.GetEnemyInRange())
                {
                    i.ApplyEffect(new Speed(target.ObjectId, i, 6, 1));
                    i.ApplyEffect(new JumpBoost(target.ObjectId, i, 10, 1));
                    i.ApplyEffect(new Stoic(target.ObjectId, i, 1, 1));
                }
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            Target.ApplyMotion(new MotionDir((t.transform.position - Target.transform.position).normalized * 20, 1f, true, 1));
            var b = GetBullet(4);
            b.Init(2f,liftstoiclevel:0);
            BulletFollowSystem.RegistObject(b,4f,1f, Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetNearestEnemy(5,false)!=null;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(5);
            b.Init(0.05f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticScaleChangeSystem.RegistObject(b,5f,2f,1f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            AddEvent(1, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.05f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
                BulletAngleSystem.RegistObject(b,2f,1f,10f,-90);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            var b = GetBullet(7);
            b.Init(0.5f, liftstoiclevel: 0);
            BulletAimSystem.RegistObject(b,1.2f,3f,Target.transform.position,12,t.transform.position);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 60f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(4);
            b.Init(0f);
            BulletStaticScaleChangeSystem.RegistObject(b,15f,0f,3f);
            b.Shoot();
            AddEvent(3f, (d) =>
            {
                foreach(var i in Monster.Monsters.Values)
                {
                    i.ApplyEffect(new DamageBoost(Target.ObjectId, i, 50, 30));
                }
                for (int startangle = 0; startangle <= 270; startangle += 90)
                {
                    var b_ = GetBullet(4);
                    b_.Init(1.2f,ec: new EffectCollection(d.Target, (EffectType.Paralysis, 0, 3)));
                    BulletOrbitSystem.RegistObject(b_,0.5f,10f,4f,90f,startangle);
                    BulletDamageOnceSystem.Regist(b_);
                    b_.Shoot();
                }
                var b = GetBullet(4);
                b.Init(1.5f);
                BulletStaticScaleChangeSystem.RegistObject(b,0f,15f,3f);
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
            TimeNeeded = 3f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle = Dt2Degree(Target.GetNearestEnemy().transform.position - Target.transform.position);
            for(int i = -20; i <= 20; i += 5)
            {
                var b = GetBullet(7);
                b.Init(1.4f, liftstoiclevel: 0,ec: new EffectCollection(Target, (EffectType.Sticky, 0, 3), (EffectType.AccuracyDecrease, 20, 5)));
                BulletAngleNonFacingSystem.RegistObject(b,0.6f,5f,8f,angle+i);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 5f;
            cd = 35f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(8,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int i = 0; i < 15; i++)
            {
                AddEvent(i * 0.2f, new TimeLineData(Target,i),(d) =>
                {
                    float angle = -12 * Mathf.Deg2Rad * d.index;
                    var t = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle))*6 + d.Target.transform.position;
                    var b = GetBullet(5);
                    b.Init(3.5f);
                    BulletProectileAimSystem.RegistObject(b,0.4f,1.5f,d.Target.transform.position,Vector3.up*55,t,1.2f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}