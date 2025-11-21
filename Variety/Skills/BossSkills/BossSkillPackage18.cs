using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
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
                float radius = 3f + (i * 2f); // 每层半径增加1单位
                AddEvent(i * 0.8f, (d) =>
                {
                    var bullet = GetBullet(6); // 6:能量球(放射)
                    var trajectory = new BulletStaticWorldScaleChange(
                        d.Target,
                        lifetime: 1f,
                        endRadius: radius,
                        pos: enemyPos,
                        startRadius: radius * 0.5f
                    );

                    var data = new BulletDataCommon(
                        d.Target,
                        new Damage_Once(),
                        2,
                        4f
                    );

                    bullet.Init(trajectory, data).Shoot();
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
            // 每个敌人生成4颗烟火弹
            for (int i = 0; i < 4; i++)
            {
                AddEvent(0.4f+i * 0.3f, (d) =>
                {
                    foreach (var enemy in enemies)
                    {
                        var bullet = GetBullet(16); // 16:烟火
                        var trajectory = new BulletAim(
                                Target,
                                lifetime: 2f,
                                worldStartPos: Target.transform.position,
                                speed: 7f,
                                aimAt: enemy,
                                radius: 0.5f
                            );

                        var data = new BulletDataSlight(
                                Target,
                                new Damage_Once(),
                                0.5f
                            );

                        bullet.Init(trajectory, data).Shoot();
                    }
                });
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
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) return;

            // 计算敌人聚集中心
            Vector3 centerPos = Vector3.zero;
            foreach (var enemy in enemies) centerPos += enemy.transform.position;
            centerPos /= enemies.Count;

            WarningCircle.Warn(centerPos, 12f, 1.5f);

            // 1.5秒后生成禁锢场
            AddEvent(1.5f, (d) =>
            {
                var field = GetBullet(4);
                var trajectory = new BulletStaticWorldScaleChange(
                    d.Target,
                    lifetime: 6f,
                    endRadius: 12f,
                    pos: centerPos
                );

                var data = new BulletDataAttract(
                    d.Target,
                    damagedt: 0.5f,
                    damagerate: 0.4f,
                    attractPower: 12f
                );

                field.Init(trajectory, data).Shoot();

                // 对范围内敌人施加眩晕
                foreach (var enemy in d.Target.GetEnemyInRange())
                {
                    if (enemy == null) continue;
                    enemy.ApplyEffect(new Stun(Target, enemy, 3f));
                }
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
            foreach (var i in enemies) WarningCircle.Warn(i.transform.position, 2, 0.5f);

            // 沿移动路径生成3个陷阱
            for (int i = 0; i < 3; i++)
            {
                AddEvent(i * 0.5f, (d) =>
                {
                    foreach (var enemy in enemies)
                    {
                        var rb = enemy.GetComponent<Rigidbody2D>();
                        Vector2 velocity = rb ? rb.velocity : Vector2.zero;
                        Vector3 trapPos = enemy.transform.position+(Vector3)rb.velocity;

                        var explosion = GetBullet(11); // 11:爆炸
                        var expTraj = new BulletStatic(
                            d.Target,
                            0.5f,
                            2f,
                            trapPos
                        );

                        var expData = new BulletDataStrike(
                            d.Target,
                            new Damage_Once(),
                            2f
                        );

                        explosion.Init(expTraj, expData).Shoot();
                    }
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
            }
            AddEvent(2f, (d) =>
            {
                foreach (var p in pos)
                {
                    var explosion = GetBullet(11);
                    var expTraj = new BulletStatic(
                        d.Target,
                        0.5f,
                        4f,
                        p
                    );

                    var expData = new BulletDataCommon(
                        d.Target,
                        new Damage_Once(),
                        2,
                        ec:ec
                    );

                    explosion.Init(expTraj, expData).Shoot();
                }
            });
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