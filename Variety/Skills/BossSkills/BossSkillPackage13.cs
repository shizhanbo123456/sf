using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
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
                foreach (var p in target.GetEnemyInRange(99999, false))
                {
                    p.ApplyEffect(new DefenseDecrease(target, p, 0.3f, 1f));
                    p.ApplyEffect(new BadLuck(target, p, 10, 1f));
                }
            }
            else
            {
                foreach (var p in target.GetEnemyInRange(99999, false))
                {
                    p.ApplyEffect(new JumpBoost(target,p,8,1));
                }
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 12f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle = Dt2Degree(Target.GetNearestEnemy(99999, false).transform.position - Target.transform.position);
            for (int i = -5; i <= 5; i += 5)
            {
                var r = (angle + i) * Mathf.Deg2Rad;
                WarningRect.Warn(Target.transform.position, Target.transform.position + new Vector3(Mathf.Cos(r), Mathf.Sin(r))*90, 0.5f, 1f);
            }
            AddEvent(1, (d) =>
            {
                for (int i = -5; i <= 5; i += 5)
                {
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 6, 15, angle + i, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var pos = Target.GetNearestEnemy(99999, false).transform.position;
            WarningCircle.Warn(pos, 2.5f, 1.1f);
            for (int i = 0; i < 5; i++)
            {
                AddEvent(1 + 0.1f * i, (d) =>
                {
                    GetBullet(6).Init(new BulletStaticWorldScaleChange(d.Target, 0.5f, 2.5f, pos, 0), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
                });
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        private Lantern lantern;
        public Skill2(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 2f;
            cd = 3f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var angle = Dt2Degree(Target.GetNearestEnemy(99999, false).transform.position - Target.transform.position) * Mathf.Deg2Rad;
            WarningRect.Warn(Target.transform.position, Target.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)), 2.5f, 2);
            AddEvent(2, (d) =>
            {
                GetBullet(14).Init(new BulletAngleNonFacing(d.Target, 6, 15, angle, 2f), new BulletDataStrike(d.Target, new Damage_Once(), 4.5f)).Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 1.5f;
            cd = 15f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            for (int i = -20; i <= 20; i += 5)
            {
                WarningRect.Warn(p + Vector3.up * 20+Vector3.right*i, p - Vector3.up * 20 + Vector3.right * i, 1.5f, 1);
            }
            AddEvent(1, (d) =>
            {
                for (int i = -20; i <= 20; i += 5)
                {
                    GetBullet(6).Init(new BulletFromTo(d.Target, 0.6f, p + Vector3.up * 20 + Vector3.right * i, p - Vector3.up * 20 + Vector3.right * i, 0.75f), new BulletDataCommon(Target, new Damage_Once(), 3.5f)).Shoot();
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 10f;
            cd = 30f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            Target.ApplyEffect(new AttackBoost(Target, Target, 0.3f, 20));
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i, (d) =>
                {
                    GetBullet(7).Init(new BulletOrbit(Target, 5, 2, 420, 0, 0.2f), new BulletDataCommon(Target, new Damage_Time(0.8f), 0.2f)).Shoot();
                });
            }
            for (float i = 0; i < 4.9f; i += 0.5f)
            {
                AddEvent(i + 5.5f, (d) =>
                {
                    var p = Dt2Degree(d.Target.GetNearestEnemy(99999, false).transform.position - d.Target.transform.position) * Mathf.Deg2Rad;
                    GetBullet(7).Init(new BulletAngleNonFacing(d.Target, 2, 40, p, 0.2f), new BulletDataCommon(d.Target, new Damage_Time(0.8f), 0.2f)).Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        private Lantern lantern;
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 3f;
            cd = 40f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[0];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int j = 0; j < 30; j++)
            {
                int i = j;
                AddEvent(i * 0.1f, (d) =>
                {
                    float a = i * 147f * Mathf.Deg2Rad;
                    Vector3 v = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                    var p = d.Target.GetNearestEnemy(99999, false).transform.position;
                    GetBullet(7).Init(new BulletProjectileAim(Target, 1.5f, d.Target.transform.position, v * 20, p, 1, 0.3f), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
                });
            }
        }
    }
}