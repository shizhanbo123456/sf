using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Damageable;
using Variety.Template;

namespace Variety.Skill.Boss16
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];

            if (lantern != null) lantern.ApplyEffect(new Burning(target, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target, t, 30, 1)));
        }
    }

    // 技能0：能量球扩散（初始储存1次）
    public class Skill0 : SkillBoss
    {
        private Lantern lantern;

        public Skill0(Target t) : base(t)
        {
            Description = "向靠近的敌人发射扩散能量球";
            TimeNeeded = 2f;
            cd = 6f;
            restoreTime = 1; // 初始储存1次

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];
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
            for (int i = 0; i < 4; i++) WarningRect.Warn(enemyPos + Angle2Vector(i * 30 + 45) * 5, enemyPos - Angle2Vector(i * 30 + 45) * 5,2.5f,0.5f);
            AddEvent(0.5f, (d) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var bullet = GetBullet(4); // 4:能量球
                    var trajectory = new BulletFromTo(
                        d.Target,
                        lifetime: 2f,
                        enemyPos + Angle2Vector(i * 30 + 45) * 5, enemyPos - Angle2Vector(i * 30 + 45) * 5, 1.2f
                    );

                    var data = new BulletDataCommon(
                        d.Target,
                        new Damage_Once(),
                        1.2f
                    );

                    bullet.Init(trajectory, data).Shoot();
                }
            });
        }
    }

    // 技能1：烟雾陷阱（初始储存0次）
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;

        public Skill1(Target t) : base(t)
        {
            Description = "在敌人路径生成烟雾陷阱，触发后造成持续伤害";
            TimeNeeded = 2.5f;
            cd = 8f;
            restoreTime = 0; // 初始不储存

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            List<Vector3>pos=new List<Vector3>();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // 预测移动路径，在前方3单位生成陷阱
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity * 1.2f;

                WarningCircle.Warn(trapPos, 4, 1f);
                pos.Add(trapPos);
            }
            AddEvent(1f, (d) =>
            {
                foreach (var p in pos)
                {
                    var smoke = GetBullet(14); // 14:雾气
                    var smokeTraj = new BulletStatic(
                        d.Target, 3f, 4f, p
                    );

                    var data = new BulletDataSlight(
                        d.Target, new Damage_Time(0.4f), 0.5f
                    );

                    smoke.Init(smokeTraj, data).Shoot();
                }
            });
        }
    }

    // 技能2：精准火球打击（初始储存1次）
    public class Skill2 : SkillBoss
    {
        private Lantern lantern;

        public Skill2(Target t) : base(t)
        {
            Description = "向单个敌人发射精准火球，附带击退";
            TimeNeeded = 1.8f;
            cd = 5f;
            restoreTime = 1; // 初始储存1次

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var targetEnemy = Target.GetNearestEnemy();
            if (targetEnemy == null) return;

            Vector3 enemyPos = targetEnemy.transform.position;
            WarningCircle.Warn(enemyPos, 1f, 0.8f);

            // 0.8秒后发射火球
            AddEvent(0.8f, (d) =>
            {
                var bullet = GetBullet(12); // 12:火球
                var trajectory = new BulletAim(d.Target,2,targetEnemy.transform.position,20,enemyPos,1.5f);

                var data = new BulletDataCommon(
                    d.Target,
                    new Damage_Once(),
                    3f
                );

                bullet.Init(trajectory, data).Shoot();
            });
        }
    }

    // 技能3：环绕诅咒（初始储存0次）
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;

        private EffectCollection ec;

        public Skill3(Target t) : base(t)
        {
            Description = "在敌人周围生成诅咒环，附加燃烧与易伤";
            TimeNeeded = 3f;
            cd = 12f;
            restoreTime = 0; // 初始不储存

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];

            ec = new EffectCollection(Target,(Effects.Burning,Target.effectController.GetFloatingAttributes().Gongji.Value,8));
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            List<Vector3>pos= new List<Vector3>();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                Vector3 enemyPos = enemy.transform.position;
                WarningCircle.Warn(enemyPos, 8f, 1.5f);
                pos.Add(enemyPos);
            }
            AddEvent(1.5f, (d) =>
            {
                foreach (var p in pos)
                {
                    var bullet = GetBullet(6); // 6:能量球(放射)
                    var trajectory = new BulletStaticWorldScaleChange(d.Target, 5f, 8,p);

                    var data = new BulletDataSlight(
                        d.Target,
                        new Damage_Time(0.5f), 0f, ec
                    );

                    bullet.Init(trajectory, data).Shoot();
                }
            });
        }
    }

    public class Skill4 : SkillBoss
    {
        private Lantern lantern;

        public Skill4(Target t) : base(t)
        {
            Description = "向多个敌人发射密集弹幕";
            TimeNeeded = 2.2f;
            cd = 7f;
            restoreTime = 1; // 初始储存1次

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(30);
        }

        protected override void OnUseSkill()
        {
            IEnumerable<Vector3>pos= Target.GetEnemyInRange().Select(t=>t.transform.position);
            foreach (var i in pos) WarningCircle.Warn(i, 1.5f, 1);
            // 每个敌人生成5颗子弹，快速连续发射
            for (int i = 0; i < 5; i++)
            {
                AddEvent(0.8f+i * 0.15f, (d) =>
                {
                    foreach (var i in pos)
                    {
                        var bullet = GetBullet(7); // 7:距离
                        var trajectory = new BulletAim(
                            d.Target,
                            lifetime: 2f,
                            worldStartPos: Target.transform.position,
                            speed: 20f,
                            aimAt: i,
                            radius: 0.7f
                        );

                        var data = new BulletDataCommon(
                            d.Target,
                            new Damage_Once(),
                            0.9f
                        );

                        bullet.Init(trajectory, data).Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5(Target t) : base(t)
        {
            cd = 36f;
            restoreTime = 0.6f;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 2) lantern = list[2];
        }
    }
}