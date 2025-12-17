using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;

namespace Variety.Skill.Boss15
{
    public class RepeatBoss : RepeatContent
    {
        public RepeatBoss() : base()
        {
            dt = 1f;
        }
        public override void Repeat(Target target)
        {
            Lantern lantern = null;
            var list = Lantern.Lanterns.Values.ToList();
            if (list.Count > 1) lantern = list[1];
            else return;
            if (lantern.Shengming > 3) target.ApplyEffect(new ArmorFortity(target.ObjectId, target, 90, 1));
            else if (lantern.Shengming == 1) target.GetEnemyInRange().ForEach(t => t.ApplyEffect(new ArmorShatter(target.ObjectId, t, 30, 1)));
        }
    }
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "环绕能量环";
            Tag = "单体、控制、持续伤害";
            Description = "锁定最近敌人位置，生成6枚环绕其飞行的能量弹，沿轨道高速旋转4秒，每0.3秒造成一次持续伤害";
            TimeNeeded = 2.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemy = Target.GetNearestEnemy();
            Vector3 enemyPos = enemy!=null?enemy.transform.position:Target.transform.position;

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
            sprite = new Vector2Int(1, 0);
            Name = "定点爆破";
            Tag = "单体、爆发";
            Description = "锁定最近敌人位置生成5范围预警圈，1秒后触发强力爆炸，对目标及周围敌人造成高额单次伤害";
            TimeNeeded = 2f;
            cd = 6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            Vector3 enemyPos = nearestEnemy?nearestEnemy.transform.position:Target.transform.position;

            WarningCircle.Warn(enemyPos, 5f, 1f);

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
            sprite = new Vector2Int(2, 0);
            Name = "引力迷雾";
            Tag = "范围、控制、爆发";
            Description = "为范围内每个敌人生成4范围预警圈，3秒后触发引力迷雾，吸附周围敌人并造成高额范围伤害";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
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
            sprite = new Vector2Int(3, 0);
            Name = "全域灼烧标记";
            Tag = "全屏、debuff、持续伤害";
            Description = "生成15范围超大预警圈，5秒蓄力后标记范围内所有敌人，使其每秒受到自身3%生命值的灼烧伤害（持续5秒），并降低20%防御（持续8秒）";
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
            sprite = new Vector2Int(4, 0);
            Name = "密集追踪火雨";
            Tag = "范围、压制、精准";
            Description = "2.5秒内分12波向范围内所有敌人发射追踪火球，每波精准锁定敌人方向，形成密集火力压制";
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
    public class Skill5 : Skill5For14_18
    {
        public Skill5() : base()
        {
            cd = 26f;
        }
    }
}