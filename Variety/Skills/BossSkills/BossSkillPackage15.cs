using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;
using Variety.Template;

namespace Variety.Skill.Boss15
{
    public class RepeatBoss : RepeatContent
    {
        private Lantern lantern;
        public RepeatBoss(Target t) : base(t)
        {
            dt = 1f;

            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];

            if (lantern != null) lantern.ApplyEffect(new Burning(target.ObjectId, lantern, (int)(lantern.Shengming * 0.02f), 999999));
        }
        protected override void Repeat()
        {
            if (!lantern) return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target.ObjectId, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target.ObjectId, t, 30, 1)));
        }
    }
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            Description = "在靠近的敌人周围生成环绕能量弹";
            TimeNeeded = 2.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemy = Target.GetNearestEnemy();
            Vector3 enemyPos = enemy.transform.position;

            for (int i = 0; i < 6; i++)
            {
                var b = GetBullet(4);
                b.Init(0.6f, liftstoiclevel: 0);
                BulletOrbitWorldSystem.RegistObject(b, 0.4f, 7f, 3f, 120, i * 60, enemyPos);
                BulletDamageTimeSystem.Regist(b,0.3f);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillCommonFor14_18
    {
        public Skill1() : base()
        {
            Description = "在敌人位置显示警告后引发爆炸";
            TimeNeeded = 2f;
            cd = 6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            if (nearestEnemy == null) return;

            Vector3 enemyPos = nearestEnemy.transform.position;

            WarningCircle.Warn(enemyPos, 5f, 1f);

            // 1秒后爆炸
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(5);
                BulletStaticSystem.RegistObject(b,4f,0.5f,enemyPos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillCommonFor14_18
    {
        public Skill2() : base()
        {
            Description = "在敌人周围生成持续伤害雾气";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                Vector3 enemyPos = enemy.transform.position;
                WarningCircle.Warn(enemyPos, 4, 3f);
                AddEvent(3f,new TimeLineData(Target,enemyPos), (d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.2f, hitback: (b, t) => Bullet.FigureAttractForce(b, t));
                    BulletStaticSystem.RegistObject(b, 6, 4, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            Description = "标记敌人并施加燃烧与防御降低";
            TimeNeeded = 2f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 15, 5f);
            AddEvent(5, (d) =>
            {
                var enemies = d.Target.GetEnemyInRange(15);
                foreach (var enemy in enemies)
                {
                    enemy.ApplyEffect(new Burning(d.Target.ObjectId, enemy, (int)(enemy.Shengming * 0.03f), 5));
                    enemy.ApplyEffect(new DefenseDecrease(d.Target.ObjectId, enemy, 0.2f, 8));
                }
            });
        }
    }
    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            Description = "向靠近的敌人发射追踪火球";
            TimeNeeded = 2.5f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for (int i = 0; i < 12; i++)
            {
                AddEvent(i * 0.2f, (d) =>
                {
                    foreach (var enemy in Target.GetEnemyInRange())
                    {
                        if (enemy == null) continue;
                        float angle = Dt2Degree(enemy.transform.position - Target.transform.position);
                        var b = GetBullet(12);
                        b.Init(0.6f,liftstoiclevel:0);
                        BulletAngleNonFacingSystem.RegistObject(b,0.8f,2,15,angle);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : Common.Skill5For14_18
    {
        public Skill5() : base()
        {
            cd = 26f;
        }
    }
}