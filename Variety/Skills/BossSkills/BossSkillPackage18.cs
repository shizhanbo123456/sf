using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss18
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];

            if (lantern != null) lantern.ApplyEffect(new Burning(target, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target, t, 30, 1)));
        }
    }
    public class Skill0 : SkillBoss
    {
        private Lantern lantern;

        public Skill0(Target t) : base(t)
        {
            Description = "在敌人周围生成多层扩散能量环";
            TimeNeeded = 2.5f;
            cd = 8f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var nearestEnemy = Target.GetNearestEnemy();
            if (nearestEnemy == null) return;
            Vector3 enemyPos = nearestEnemy.transform.position;
            WarningCircle.Warn(enemyPos, 3f, 0.8f);
            // 生成3层能量环，逐层扩散
            for (int i = 0; i < 3; i++)
            {
                AddEvent(i * 0.8f,new TimeLineData(Target,i), (d) =>
                {
                    float radius = 3f + (d.index * 2f);
                    var b = GetBullet(6);
                    b.Init(4);
                    BulletStaticScaleChangeSystem.RegistObject(b,radius*0.5f,radius,1f,enemyPos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }

    // 技能1：追踪烟火弹幕（初始储存1次）
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;

        public Skill1(Target t) : base(t)
        {
            Description = "向多个敌人发射带烟火特效的追踪弹幕";
            TimeNeeded = 2f;
            cd = 6f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(30);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange().Select(t=> 
            {
                var p = t.transform.position;
                return Target.transform.position+(p-Target.transform.position).normalized*30; 
            });
            foreach (var i in enemies) WarningRect.Warn(Target.transform.position, i, 1f, 0.4f);

            for (int i = 0; i < 4; i++)
            {
                foreach (var enemy in enemies)
                {
                    AddEvent(0.4f + i * 0.3f, new TimeLineData(Target,enemy),(d) =>
                    {
                        var b = GetBullet(16);
                        b.Init(0.5f);
                        BulletAimSystem.RegistObject(b, 0.5f, 2f, d.Target.transform.position, 7f,d.pos);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    });
                }
            }
        }
    }

    // 技能2：范围禁锢场（初始储存0次）
    public class Skill2 : SkillBoss
    {
        private Lantern lantern;

        public Skill2(Target t) : base(t)
        {
            Description = "生成禁锢场，使范围内敌人无法移动并持续受伤";
            TimeNeeded = 3f;
            cd = 15f;
            restoreTime = 0;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var e = Target.GetNearestEnemy();
            if (!e) return;
            WarningCircle.Warn(e.transform.position, 12f, 1.5f);
            e.ApplyEffect(new Slowness(Target, e, 3, 3f));
            // 1.5秒后生成禁锢场
            AddEvent(1.5f, new TimeLineData(Target,e.transform.position),(d) =>
            {
                var b = GetBullet(3);
                b.Init(0.1f, hitback: (b, t) => Bullet.FigureAttractForce(b,t));
                BulletStaticScaleChangeSystem.RegistObject(b,0,12,6,d.pos);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }

    public class Skill3 : SkillBoss
    {
        private Lantern lantern;

        public Skill3(Target t) : base(t)
        {
            Description = "在敌人路径生成连环爆炸陷阱";
            TimeNeeded = 2.5f;
            cd = 12f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i.transform, 2, 1f);
                AddEvent(1f, new TimeLineData(i),(d) =>
                {
                    var rb = d.Target.GetComponent<Rigidbody2D>();
                    Vector2 velocity = rb ? rb.velocity : Vector2.zero;
                    Vector3 trapPos = d.Target.transform.position + (Vector3)rb.velocity;

                    var b = GetBullet(11);
                    b.Init(2);
                    BulletStaticSystem.RegistObject(b, 2f, 0.5f, trapPos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillBoss
    {
        private Lantern lantern;
        private EffectCollection ec;
        public Skill4(Target t) : base(t)
        {
            Description = "标记敌人并在延迟后引发爆发伤害";
            TimeNeeded = 3f;
            cd = 10f;
            restoreTime = 0;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];

            ec = new EffectCollection(Target, (Effects.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value * 0.05f, 4f));
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            var pos = enemies.Select(t => t.transform.position);

            foreach (var p in pos)
            {
                WarningCircle.Warn(p, 4, 2);
                AddEvent(2f, new TimeLineData(Target, p),(d) =>
                {
                    var b = GetBullet(11);
                    b.Init(2, ec: ec);
                    BulletStaticSystem.RegistObject(b, 4f, 0.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5(Target t) : base(t)
        {
            cd = 72f;
            restoreTime = 1;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 4) lantern = list[4];
        }
    }
}