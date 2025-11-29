using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss12
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 0.3f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            var b = GetBullet(7);
            b.Init(1.8f);
            BulletFollowSystem.RegistObject(b,0.4f,0.25f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 0.25f, true, 1, true, true));
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
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int a = -10; a <= 10; a += 10)
            {
                var b = GetBullet(7);
                b.Init(0.4f,liftstoiclevel:0);
                BulletAngleSystem.RegistObject(b,0.3f,1.5f,5,a);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
            AddEvent(0.5f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;

                var b = GetBullet(7);
                b.Init(2.5f);
                BulletFollowSystem.RegistObject(b,1,0.25f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                d.Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1, true, true));
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
            WarningCircle.Warn(Target.transform.position, 3f, 1f);
            for (int i = 0; i < 3; i++)
            {
                AddEvent(1f+i * 0.3f, (d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.5f);
                    BulletStaticScaleChangeSystem.RegistObject(b,1,3,0.3f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            Target.ApplyEffect(new Speed(Target, Target, 3f, 12f));
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(8, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(5);
            b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticScaleChangeSystem.RegistObject(b,4,0,1f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionStatic(0.7f, true, 1, true, true));
            AddEvent(0.8f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;
                var b = GetBullet(14);
                b.Init(4,liftstoiclevel:2);
                BulletFollowSystem.RegistObject(b,0.4f,0.25f,d.Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1, true, true));
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 1f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 1, true, 1, true, true));
            for (int i = 0; i < 4; i++)
            {
                AddEvent(i * 0.25f,new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(14);
                    b.Init(1.2f);
                    BulletOrbitSystem.RegistObject(b,0.5f,0.25f,1.3f,(d.index % 2 == 0) ? 360 : -360, (d.index % 2 == 0) ? -135 : 135);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 5f;
            cd = 32f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 40, 0.25f, true, 1, true, true));
            var t = Target.GetNearestEnemy();
            if (!t) return;
            Vector3 p=t.transform.position;
            AddEvent(0.25f,new TimeLineData(Target,p), (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(5f, true, 1, true, true));
                WarningCircle.Warn(d.pos, 2, 0.6f);
            });
            for (int i = 0; i < 36; i++)
            {
                var angle = i * 20f * Mathf.Deg2Rad;
                AddEvent(0.9f + i * 0.1f,new TimeLineData(Target,p), (d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletProectileAimSystem.RegistObject(b,0.7f,2,d.Target.transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 20, d.pos,1.6f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}