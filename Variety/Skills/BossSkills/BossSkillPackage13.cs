using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss13
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        protected override void Repeat()
        {
            if (lantern.TimeOfDie<0.01f)
            {
                foreach (var p in target.GetEnemyInRange())
                {
                    p.ApplyEffect(new DefenseDecrease(target.ObjectId, p, 0.3f, 1f));
                    p.ApplyEffect(new BadLuck(target.ObjectId, p, 10, 1f));
                }
            }
            else
            {
                foreach (var p in target.GetEnemyInRange())
                {
                    p.ApplyEffect(new JumpBoost(target.ObjectId,p,8,1));
                }
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle = Dt2Degree(Target.GetNearestEnemy().transform.position - Target.transform.position);
            for (int i = -5; i <= 5; i += 5)
            {
                var r = (angle + i) * Mathf.Deg2Rad;
                WarningRect.Warn(Target.transform.position, Target.transform.position + new Vector3(Mathf.Cos(r), Mathf.Sin(r))*90, 0.5f, 1f);
            }
            AddEvent(1, new TimeLineData(Target,(int)angle),(d) =>
            {
                for (int i = -5; i <= 5; i += 5)
                {
                    var b = GetBullet(14);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,6,15,d.index+i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;
        public Skill1() : base()
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse(Target Target)
        {
            return lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            if (!t) return;
            var pos = t.transform.position;
            WarningCircle.Warn(pos, 2.5f, 1.1f);
            for (int i = 0; i < 5; i++)
            {
                AddEvent(1 + 0.1f * i,new TimeLineData(Target,pos), (d) =>
                {
                    var b = GetBullet(6);
                    b.Init(1.5f);
                    BulletStaticScaleChangeSystem.RegistObject(b,0,2.5f,0.5f,d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        private Lantern lantern;
        public Skill2() : base()
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse(Target Target)
        {
            return lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            if (!t) return;
            var angle = Dt2Degree(t.transform.position - Target.transform.position);
            var b = GetBullet(6);
            b.Init(4.5f);
            BulletAngleNonFacingSystem.RegistObject(b, 2f, 6, 15, angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;
        public Skill3() : base()
        {
            Description = "";
            TimeNeeded = 1.5f;
            cd = 15f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse(Target Target)
        {
            return lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var t = Target.GetNearestEnemy();
            if (!t) return;
            var p = t.transform.position;
            for (int i = -20; i <= 20; i += 5)
            {
                WarningRect.Warn(p + Vector3.up * 20+Vector3.right*i, p - Vector3.up * 20 + Vector3.right * i, 1.5f, 1);
            }
            AddEvent(1, new TimeLineData(Target,p),(d) =>
            {
                for (int i = -20; i <= 20; i += 5)
                {
                    var b = GetBullet(10);
                    b.Init(3.5f);
                    BulletFromToSystem.RegistObject(b,0.75f,0.6f,d.pos + Vector3.up * 20 + Vector3.right * i, d.pos- Vector3.up * 20 + Vector3.right * i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        public Skill4() : base()
        {
            Description = "";
            TimeNeeded = 10f;
            cd = 30f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse(Target Target)
        {
            return lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyEffect(new AttackBoost(Target.ObjectId, Target, 0.3f, 20));
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i, (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(0.2f);
                    BulletOrbitSystem.RegistObject(b,0.2f,5,2,420,0);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i + 5.5f, (d) =>
                {
                    var p = Dt2Degree(d.Target.GetNearestEnemy().transform.position - d.Target.transform.position);
                    var b = GetBullet(12);
                    b.Init(0.6f);
                    BulletAngleNonFacingSystem.RegistObject(b,0.2f,2,40,p);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        private Lantern lantern;
        public Skill5() : base()
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 40f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse(Target Target)
        {
            return lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int j = 0; j < 30; j++)
            {
                AddEvent(j * 0.1f,new TimeLineData(Target,j), (d) =>
                {
                    float a = d.index * 147f * Mathf.Deg2Rad;
                    Vector3 v = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                    var t = d.Target.GetNearestEnemy();
                    if (!t) return;
                    var p =t.transform.position;
                    var b = GetBullet(7);
                    b.Init(1.5f);
                    BulletProectileAimSystem.RegistObject(b,0.8f,1.5f,d.Target.transform.position, v * 20, p,1f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}