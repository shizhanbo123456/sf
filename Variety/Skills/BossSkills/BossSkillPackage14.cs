using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;

namespace Variety.Skill.Boss14
{
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "迷雾缠身";
            Tag = "范围、减速、持续伤害";
            Description = "在范围内每个敌人周围生成预警圈，2秒后触发持续伤害雾气，附带2点减速效果（持续5秒），持续灼烧敌人";
            TimeNeeded = 2f;
            cd = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var ec = new EffectCollection(Target, (EffectType.Slowness, 2f, 5f));
            var enemies = Target.GetEnemyInRange();
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i.transform.position, 3, 2);
                AddEvent(2, new TimeLineData(Target,i.transform.position),(d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.4f, ec: ec);
                    BulletStaticSystem.RegistObject(b, 5, 2, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.2f);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillCommonFor14_18
    {
        public Skill1():base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "追踪火球术";
            Tag = "单体、精准、连击";
            Description = "锁定最近敌人位置，分3波发射追踪火球，每波间隔0.6秒，火球精准飞向目标，形成连续打击";
            TimeNeeded = 2.5f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy(); // 获取最近敌人

            for (int wave = 0; wave < 3; wave++)
            {
                AddEvent(wave * 0.6f,new TimeLineData(Target,nearestEnemy!=null?nearestEnemy.transform.position:Target.transform.position-Vector3.up), (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(2.2f);
                    BulletAimSystem.RegistObject(b,0.7f,4f,d.Target.transform.position,6f,d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }

    public class Skill2 : SkillCommonFor14_18
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "能量爆发波";
            Tag = "范围、爆发、封锁";
            Description = "锁定最近敌人位置生成4范围预警圈，2秒后从中心向8个方向发射贯穿子弹，形成环形爆发，封锁周围区域";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemy = Target.GetNearestEnemy();
            Vector3 centerPos = enemy!=null?enemy.transform.position:Target.transform.position;

            WarningCircle.Warn(centerPos, 4f, 2f);

            AddEvent(2f, (d) =>
            {
                for (int i = 0; i < 8; i++)
                {
                    var b = GetBullet(4);
                    b.Init(3);
                    BulletFromToSystem.RegistObject(b,0.8f,0.5f,centerPos+Angle2Vector(i*45), centerPos + Angle2Vector(i * 45+180));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "灼烧破甲标记";
            Tag = "单体、debuff、持续伤害";
            Description = "为范围内每个敌人生成预警圈，1秒后施加标记，使敌人持续燃烧（每秒造成自身50%攻击力伤害，持续10秒）并降低20%防御（持续10秒）";
            TimeNeeded = 2f;
            cd = 15f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var ec = new EffectCollection(Target,
                (EffectType.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value * 0.5f, 10),
                (EffectType.DefenseDecrease, 0.2f, 10));
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
            foreach (var i in enemies)
            {
                WarningCircle.Warn(i.transform.position, 2f, 1);
                AddEvent(1, new TimeLineData(Target,i.transform.position),(d) =>
                {
                    var b = GetBullet(16);
                    b.Init(0.2f, liftstoiclevel: 0, ec: ec);
                    BulletStaticSystem.RegistObject(b, 2f, 10f, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.2f);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "路径预判陷阱";
            Tag = "控制、突袭、预判";
            Description = "预测敌人移动路径，在其前方5单位生成陷阱预警圈，1秒后陷阱触发爆炸，精准打击移动中的敌人";
            TimeNeeded = 2.5f;
            cd = 9f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if(enemies.Count == 0)enemies.Add(Target);
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                // 预测敌人移动路径，在前方2单位生成陷阱
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity.normalized * 5f;

                WarningCircle.Warn(trapPos, 2f, 1f);
                AddEvent(1f, new TimeLineData(Target,trapPos),(d) =>
                {
                    var b = GetBullet(11);
                    b.Init(2.4f);
                    BulletStaticSystem.RegistObject(b, 2f, 0.5f, d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : Skill5For14_18
    {
        public Skill5():base()
        {
            cd = 21f;
        }
    }
}