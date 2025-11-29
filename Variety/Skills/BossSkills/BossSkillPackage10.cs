using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;
using XLua.Cast;

namespace Variety.Skill.Boss10
{
    public class Skill0 : SkillBoss
    {
        private static List<Vector3> startVelocity = new List<Vector3>()
        {
            new Vector3(0,4),
            new Vector3(0,6),
            new Vector3(0,9),
        };
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            foreach (var v in startVelocity) {
                var b = GetBullet(16);
                b.Init(1.2f);
                BulletProectileSystem.RegistObject(b, 0.8f, 3f, Target.transform.position, v+front*20);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 22f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            WarningCircle.Warn(Target.transform.position, 3, 1.2f);
            AddEvent(1.2f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(1.2f);
                BulletStaticSystem.RegistObject(b,3f,0.6f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                Target.ApplyEffect(new AttackBoost(d.Target.ObjectId, Target, 0.3f, 10f));
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
        public override bool CanUse(Target Target)
        {
            return Target.GetEnemyInRange(12, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up*12, 0.5f, true, 1, true, true));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.5f, true, 1, true,true));
                for(int i = -15; i >= -75; i -= 15)
                {
                    var b = GetBullet(7);
                    b.Init(0.5f);
                    BulletAngleSystem.RegistObject(b,0.3f,1f,15f,i);
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
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(7);
            b.Init(0.6f,liftstoiclevel:0);
            BulletOrbitSystem.RegistObject(b,0.7f,10,6,120,0);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            b = GetBullet(7);
            b.Init(0.6f, liftstoiclevel: 0);
            BulletOrbitSystem.RegistObject(b, 0.7f, 10, 6, 120, 180);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            WarningRect.Warn(Target.transform.position, Target.transform.position + front * 20, 4, 1.2f);
            AddEvent(1.2f,new TimeLineData(Target, Target.transform.position + front * 20) ,(d) =>
            {
                for(int j=0;j<5;j++)
                {
                    var b = GetBullet(7);
                    b.Init(0.6f);
                    BulletProectileAimSystem.RegistObject(b,0.7f,2f,d.Target.transform.position,BulletSystemCommon.AngleToVector(j*72f)*20, d.pos,1.4f);
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
            TimeNeeded = 2f;
            cd = 40f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyMotion(new MotionDir(Vector2.up * 10, 1f, true, 1, true, true));
            AddEvent(1f, (d) =>
            {
                var t = Target.GetNearestEnemy();
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 4, 0.25f, true, 1, true, true));
            });
            AddEvent(1.25f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.3f, true, 1, true, true));
                var t = d.Target.GetNearestEnemy();
                var b = GetBullet(11);
                b.Init(4f);
                BulletStaticSystem.RegistObject(b,3f,0.3f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
}