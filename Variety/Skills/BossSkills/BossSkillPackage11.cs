using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss11
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        protected override void Repeat()
        {
            if (lantern.TimeOfDie<0.01f)
            {
                foreach (var p in target.GetEnemyInRange(99999, false))
                {
                    p.ApplyEffect(new ArmorFortity(target, p, 20, 1));
                }
                target.ApplyEffect(new Luck(target, target, 10, 1));
            }
            else
            {
                target.ApplyEffect(new AttackDecrease(target, target, 0.5f, 1));
            }
        }
    }
    public class Skill0 : SkillBoss
    {
        public Skill0(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            WarningRect.Warn(p, p + Vector3.up * 5, 1.3f, 1f);
            AddEvent(1, (d) =>
            {
                GetBullet(7).Init(new BulletFromTo(d.Target, 0.3f, p + Vector3.up * 5, p, 0.4f), new BulletDataCommon(d.Target, new Damage_Once(), 0.8f)).Shoot();
            });
            AddEvent(1.3f, (d) =>
            {
                GetBullet(11).Init(new BulletStatic(d.Target, 0.5f, 1f, p), new BulletDataCommon(d.Target, new Damage_Once(), 0.8f)).Shoot();
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;
        public Skill1(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 4f;
            cd = 30f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for (int i = 0; i < 30; i++)
            {
                int j = i;
                AddEvent(i * 0.1f, (d) =>
                {
                    float a = (j * 129) % 90 + 45;
                    Vector3 v = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                    var p = Target.GetNearestEnemy(99999, false).transform.position;
                    GetBullet(4).Init(new BulletProjectileAim(d.Target, 1.5f, d.Target.transform.position, v * 30, p, 1f, 0.3f), new BulletDataSlight(d.Target, new Damage_Once(), 0.5f)).Shoot();
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
            TimeNeeded = 0.5f;
            cd = 3f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            WarningRect.Warn(p + Vector3.left * 5, p + Vector3.right * 5, 2, 1f);
            AddEvent(1f, (d) =>
            {
                GetBullet(7).Init(new BulletFromTo(d.Target, 1f, p + Vector3.left * 5, p + Vector3.right * 5, 1f), new BulletDataCommon(d.Target, new Damage_Once(), 1.5f)).Shoot();
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;
        public Skill3(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 7f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            WarningRect.Warn(p + Angle2Vector(45)*5, p + Angle2Vector(225)*5, 2, 1f);
            WarningRect.Warn(p + Angle2Vector(-45)*5, p + Angle2Vector(135)*5, 2, 1f);
            AddEvent(1, (d) =>
            {
                GetBullet(7).Init(new BulletFromTo(d.Target, 1f, p + Angle2Vector(45)*5, p + Angle2Vector(225)*5, 1f), new BulletDataCommon(d.Target, new Damage_Once(), 1f)).Shoot();
                GetBullet(7).Init(new BulletFromTo(d.Target, 1f, p + Angle2Vector(-45)*5, p + Angle2Vector(135)*5, 1f), new BulletDataCommon(d.Target, new Damage_Once(), 1f)).Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        public Skill4(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 0.5f;
            cd = 28f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            for (int i = 0; i < 18; i++)
            {
                WarningRect.Warn(p + Angle2Vector(i * 18 + 45)*10, p + Angle2Vector(i * 18 + 225)*10, 0.2f, 2.5f - 0.1f * i);
            }
            AddEvent(2.5f, (d) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    GetBullet(12).Init(new BulletFromTo(d.Target, 0.4f, p + Angle2Vector(i * 18 + 45)*9, p + Angle2Vector(i * 18 + 225)*9, 0.2f), new BulletDataCommon(d.Target, new Damage_Once(), 2.5f)).Shoot();
                }
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        private Lantern lantern;
        private static List<(Vector3, Vector3)> Pos = new List<(Vector3, Vector3)>()
        {
            (new Vector3( -9 , -11 ),new Vector3( -2 , 13 )),
            (new Vector3( 7 , -5 ),new Vector3( 10 , 18 )),
            (new Vector3( -12 , 8 ),new Vector3( -7 , -3 )),
            (new Vector3( -11 , 11 ),new Vector3( -5 , -9 )),
            (new Vector3( -18 , 3 ),new Vector3( 4 , -4 )),
            (new Vector3( -5 , -5 ),new Vector3( -14 , -17 )),
            (new Vector3( 4 , 10 ),new Vector3( -15 , -15 )),
            (new Vector3( -17 , -6 ),new Vector3( -11 , -16 )),
            (new Vector3( 1 , -13 ),new Vector3( 20 , -19 )),
            (new Vector3( -7 , -17 ),new Vector3( 0 , 15 )),
            (new Vector3( -14 , -19 ),new Vector3( -20 , 4 )),
            (new Vector3( 7 , -10 ),new Vector3( 4 , 14 )),
            (new Vector3( 0 , 9 ),new Vector3( 13 , 10 )),
            (new Vector3( 2 , -7 ),new Vector3( -16 , -20 )),
            (new Vector3( -3 , -4 ),new Vector3( 8 , 17 )),
            (new Vector3( -2 , 0 ),new Vector3( -6 , -17 )),
            (new Vector3( 3 , -4 ),new Vector3( 5 , 20 )),
            (new Vector3( 4 , -16 ),new Vector3( -18 , -6 )),
            (new Vector3( -13 , -18 ),new Vector3( 3 , 12 )),
            (new Vector3( 8 , 3 ),new Vector3( 4 , -8 )),
            (new Vector3( -20 , -17 ),new Vector3( -5 , 12 )),
            (new Vector3( -15 , 1 ),new Vector3( -20 , 2 )),
            (new Vector3( 3 , -12 ),new Vector3( -12 , -18 )),
            (new Vector3( 5 , 19 ),new Vector3( -19 , 7 )),
            (new Vector3( -2 , 18 ),new Vector3( 14 , 16 )),
            (new Vector3( -9 , 17 ),new Vector3( -13 , 3 )),
            (new Vector3( 8 , 15 ),new Vector3( -14 , 8 )),
            (new Vector3( -7 , 16 ),new Vector3( 5 , -8 )),
            (new Vector3( 17 , 5 ),new Vector3( 4 , -5 )),
            (new Vector3( 10 , 11 ),new Vector3( 7 , 2 )),
            (new Vector3( 4 , 9 ),new Vector3( 12 , 11 )),
            (new Vector3( 0 , 17 ),new Vector3( -13 , -4 )),
            (new Vector3( 20 , 10 ),new Vector3( 5 , 10 )),
            (new Vector3( -18 , -8 ),new Vector3( 2 , -11 )),
            (new Vector3( -11 , -14 ),new Vector3( -14 , 12 )),
            (new Vector3( -11 , 3 ),new Vector3( -16 , -18 )),
            (new Vector3( -17 , 3 ),new Vector3( -17 , 8 )),
            (new Vector3( -17 , -15 ),new Vector3( -13 , -3 )),
            (new Vector3( 7 , -19 ),new Vector3( -12 , -7 )),
            (new Vector3( 20 , -17 ),new Vector3( -8 , 5 )),
        };
        public Skill5(Target t) : base(t)
        {
            Description = "";
            TimeNeeded = 4f;
            cd = 25f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 0) lantern = list[^1];
        }
        public override bool CanUse()
        {
            return base.CanUse() && lantern && lantern.TimeOfDie < 0.01f;
        }
        protected override void OnUseSkill()
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var p = Target.GetNearestEnemy(99999, false).transform.position;
            for (int i = 0; i < 40; i++)
            {
                WarningRect.Warn(p + Pos[i].Item1, p + Pos[i].Item2, 0.2f, 3);
            }
            AddEvent(3, (d) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    GetBullet(7).Init(new BulletFromTo(d.Target, 0.2f, p + Pos[i].Item1, p + Pos[i].Item2, 0.2f), new BulletDataCommon(d.Target, new Damage_Once(), 2.5f)).Shoot();
                }
            });
        }
    }
}