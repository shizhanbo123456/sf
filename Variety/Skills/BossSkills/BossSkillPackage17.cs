using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss17
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];

            if (lantern != null) lantern.ApplyEffect(new Burning(target, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target, t, 30, 1)));
        }
    }

    // 技能0：速度预测火球（根据敌人速度预判位置）
    public class Skill0 : SkillBoss
    {
        private Lantern lantern;

        public Skill0(Target t) : base(t)
        {
            Description = "根据敌人移动速度预测位置发射火球";
            TimeNeeded = 2.2f;
            cd = 7f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            List<(Vector3, Vector3)> pos = new List<(Vector3, Vector3)>();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                var rb = enemy.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                // 预测0.8秒后的位置（火球飞行时间）
                Vector3 predictPos = enemy.transform.position + (Vector3)(rb.velocity * 1.2f);
                Vector3 startPos = (Target.transform.position - enemy.transform.position).normalized * 20 + enemy.transform.position;
                pos.Add((startPos, predictPos));

                WarningRect.Warn(pos[^1].Item1, pos[^1].Item2, 1.2f, 0.8f);
            }
            AddEvent(0.8f, (d) =>
            {
                foreach (var i in pos)
                {
                    var b = GetBullet(12);
                    b.Init(2);
                    BulletFromToSystem.RegistObject(b,0.6f,0.5f,i.Item1,i.Item2);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        private Lantern lantern;
        public Skill1(Target t) : base(t)
        {
            Description = "在敌人路径生成地雷，移动越快伤害越高";
            TimeNeeded = 2.5f;
            cd = 10f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
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
                var rb = enemy.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                // 根据速度计算地雷生成位置
                Vector2 velocity = rb.velocity;
                float speed = velocity.magnitude;
                Vector3 spawnPos = enemy.transform.position + (Vector3)(velocity.normalized * Mathf.Min(speed * 0.8f, 12f));
                pos.Add(spawnPos);
            }
            foreach (var i in pos) WarningCircle.Warn(i, 3, 1);
            AddEvent(1, (d) =>
            {
                foreach (var i in pos)
                {
                    var b = GetBullet(6);
                    b.Init(3);
                    BulletStaticSystem.RegistObject(b,2.5f,1.5f,i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }

    public class Skill2 : SkillBoss
    {
        private Lantern lantern;
        private EffectCollection ec;

        public Skill2(Target t) : base(t)
        {
            Description = "生成禁锢环，减缓敌人速度并造成持续伤害";
            TimeNeeded = 3f;
            cd = 12f;
            restoreTime = 0;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];

            ec = new EffectCollection(Target, (Effects.Slowness, 3, 5));
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
            WarningCircle.Warn(enemyPos, 3, 1);
            AddEvent(1,new TimeLineData(Target,enemyPos), (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.6f,liftstoiclevel:0,ec:ec);
                BulletStaticSystem.RegistObject(b,3,5,d.pos);
                BulletDamageTimeSystem.Regist(b,0.3f);
                b.Shoot();
            });
        }
    }

    // 技能3：分向速度弹幕（根据敌人移动方向发射弹幕）
    public class Skill3 : SkillBoss
    {
        private Lantern lantern;

        public Skill3(Target t) : base(t)
        {
            Description = "沿敌人移动方向两侧发射弹幕";
            TimeNeeded = 2f;
            cd = 8f;
            restoreTime = 1;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3)&&Target.GetNearestEnemy(20);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var i in enemies) WarningCircle.Warn(i.transform, 2f, 5f);
            foreach (var enemy in enemies)
            {
                var rb = enemy.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                // 每侧发射5颗子弹
                for (int i = 0; i < 5; i++)
                {
                    float delay = i * 0.2f;
                    // 左侧弹幕
                    AddEvent(5+delay,new TimeLineData(Target,rb.velocity), (d) =>
                    {
                        var dir=d.pos.magnitude > 0.1f ? d.pos.normalized : Vector3.right;
                        Vector3 side1 = new Vector2(-dir.y, dir.x);
                        Vector3 side2 = new Vector2(dir.y, -dir.x);
                        var b = GetBullet(7);
                        b.Init(0.6f);
                        BulletFromToSystem.RegistObject(b,0.4f,1f,enemy.transform.position + side1 * 5, enemy.transform.position - side1 * 2);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();

                        b = GetBullet(7);
                        b.Init(0.6f);
                        BulletFromToSystem.RegistObject(b, 0.4f, 1f, enemy.transform.position + side2 * 5, enemy.transform.position - side2 * 2);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    });
                }
            }
        }
    }

    public class Skill4 : SkillBoss
    {
        private Lantern lantern;

        public Skill4(Target t) : base(t)
        {
            Description = "发射跟随敌人速度变化的追踪导弹";
            TimeNeeded = 2.8f;
            cd = 15f;
            restoreTime = 0;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
        }

        public override bool CanUse()
        {
            return base.CanUse() && (lantern == null || lantern.Shengming < 3);
        }

        protected override void OnUseSkill()
        {
            var enemies = Target.GetEnemyInRange();

            for (int i = 0; i < 7;i++)
            {
                AddEvent(i * 0.4f, (d) =>
                {
                    foreach (var enemy in enemies)
                    {
                        var b = GetBullet(7);
                        b.Init(0.8f);
                        BulletProectileAimSystem.RegistObject(b,0.4f,2f, d.Target.transform.position, Vector3.up * 20, enemy.transform.position,1.2f);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5(Target t) : base(t)
        {
            cd = 55f;
            restoreTime = 0.8f;
        }
        protected override void GetLantern()
        {
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 3) lantern = list[3];
        }
    }
}