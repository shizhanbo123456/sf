using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss7
{
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
            WarningCircle.Warn(Target.transform.position, 4, 1);
            Target.ApplyMotion(new MotionDir(new Vector2(0, 20), 0.5f, true, 1));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.3f, true, 1));
            });
            AddEvent(0.8f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(Vector2.down * 50, 0.2f, true, 1));
            });
            AddEvent(1, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(new Vector2(0, 10), 0.2f, true, 1));
                var b = GetBullet(11);
                b.Init(1.5f);
                BulletStaticSystem.RegistObject(b,4f,0.5f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 0.5f, true, 1));
            for(int offset=-10;offset<=10;offset+=5)
            {
                var b = GetBullet(7);
                b.Init(1.5f);
                BulletAngleSystem.RegistObject(b,0.7f,3,5,offset);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
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
            var p = Target.transform.position;
            Target.ApplyMotion(new MotionVelocityChange(Vector2.up*10,true,1));
            WarningCircle.Warn(p, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.right*5, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.left*5, 2, 0.5f);
            AddEvent(0.5f, new TimeLineData(Target,p),(d) =>
            {
                for(int i = -1; i <= 1; i++)
                {
                    var b = GetBullet(11);
                    b.Init(1.4f);
                    BulletStaticSystem.RegistObject(b,2f,0.3f,d.pos+new Vector3(i*5,0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 25f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(3, false).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(5);
            b.Init(0.1f, hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletFollowSystem.RegistObject(b,3,2,Target);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionVelocityLerp(Vector2.up * 10, Vector2.up * 5, 2, true, 1));
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.1f);
                BulletDirSystem.RegistObject(b,3f,2f,10f,new Vector2(2,-1));
                BulletDamageTimeSystem.Regist(b,0.2f);
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
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(2, true, 1));
            var b = GetBullet(5);
            b.Init(0.1f,  hitback: (b, t) => Bullet.FigureAttractForce(b, t));
            BulletStaticScaleChangeSystem.RegistObject(b,8f,3f,2f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionVelocityLerp(Vector2.up * 10, Vector2.up * 5, 2, true, 1));
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.1f);
                BulletDirSystem.RegistObject(b,3f,3f,8f, Vector2.down);
                BulletDamageTimeSystem.Regist(b, 0.2f);
                b.Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 60f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(5, true, 1));
            var b = GetBullet(5);
            b.Init(0f);
            BulletStaticScaleChangeSystem.RegistObject(b,20,0,5);
            b.Shoot();
            AddEvent(5, (d) =>
            {
                var b = GetBullet(5);
                b.Init(2,liftstoiclevel:0, ec: new EffectCollection(d.Target, (EffectType.ArmorShatter, 20, 30)));
                BulletStaticScaleChangeSystem.RegistObject(b,0f,20f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                var t=Target.GetPartnerInRange(999999, false);
                foreach(var i in t)
                {
                    i.ApplyEffect(new DamageBoost(d.Target.ObjectId, i, 30, 30));
                    i.ApplyEffect(new Luck(d.Target.ObjectId, i, 20, 30));
                }
            });
        }
    }
}