using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Skill.Common;

namespace Variety.Skill.Boss16
{
    public class Skill0 : SkillCommonFor14_18
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "扩散能量波";
            Tag = "范围、封锁";
            Description = "锁定最近敌人位置，生成4组交叉预警区域，0.5秒后发射4枚贯穿能量球，沿交叉轨迹穿行，封锁目标周围空间";
            TimeNeeded = 2f;
            cd = 6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var nearestEnemy = Target.GetNearestEnemy();
            Vector3 enemyPos = nearestEnemy?nearestEnemy.transform.position:Target.transform.position+Vector3.down;
            for (int i = 0; i < 4; i++) WarningRect.Warn(enemyPos + Angle2Vector(i * 30 + 45) * 10, enemyPos - Angle2Vector(i * 30 + 45) * 10,2.5f,0.5f);
            AddEvent(0.5f,new TimeLineData(Target,enemyPos), (d) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var b = GetBullet(4);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b,1.2f,2f,d.pos + Angle2Vector(i * 30 + 45) * 10, d.pos - Angle2Vector(i * 30 + 45) * 10);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill1 : SkillCommonFor14_18
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "烟雾追踪陷阱";
            Tag = "控制、持续伤害、预判";
            Description = "预测范围内敌人移动路径，在其前方生成烟雾陷阱预警圈，1秒后陷阱激活，持续4秒造成高频伤害，限制敌人走位";
            TimeNeeded = 2.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            foreach (var enemy in enemies)
            {
                Vector3 trapPos = enemy.transform.position + (Vector3)enemy.GetComponent<Rigidbody2D>().velocity * 1.5f;

                WarningCircle.Warn(trapPos, 4, 1f);
                AddEvent(1f, new TimeLineData(Target,trapPos),(d) =>
                {
                    var b = GetBullet(14);
                    b.Init(0.5f, liftstoiclevel: 0);
                    BulletStaticSystem.RegistObject(b, 3, 4, d.pos);
                    BulletDamageTimeSystem.Regist(b, 0.3f);
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
            Name = "精准火球术";
            Tag = "单体、爆发、精准";
            Description = "锁定最近敌人位置生成小型预警圈，0.8秒后发射一枚高速精准火球，对单个目标造成高额单次伤害";
            TimeNeeded = 1.8f;
            cd = 5f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var targetEnemy = Target.GetNearestEnemy();
            Vector3 enemyPos = targetEnemy?targetEnemy.transform.position:Target.transform.position+Target.Front;
            WarningCircle.Warn(enemyPos, 1f, 0.8f);

            // 0.8秒后发射火球
            AddEvent(0.8f, (d) =>
            {
                var b = GetBullet(12);
                b.Init(3f, liftstoiclevel: 2);
                BulletAimSystem.RegistObject(b,1.5f,2f,d.Target.transform.position,20,enemyPos);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill3 : SkillCommonFor14_18
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "诅咒灼烧环";
            Tag = "范围、debuff、持续伤害";
            Description = "为范围内每个敌人生成8范围预警圈，1.5秒后触发诅咒环，弹幕从无到有逐渐扩大，附带高额燃烧伤害（基于自身攻击力，持续8秒）";
            TimeNeeded = 3f;
            cd = 12f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
            List<Vector3>pos= new List<Vector3>();
            foreach (var enemy in enemies)
            {
                Vector3 enemyPos = enemy.transform.position;
                WarningCircle.Warn(enemyPos, 8f, 1.5f);
                pos.Add(enemyPos);
            }
            AddEvent(1.5f, (d) =>
            {
                foreach (var p in pos)
                {
                    var b = GetBullet(6);
                    b.Init(0.2f,liftstoiclevel: 0, new EffectCollection(d.Target, (EffectType.Burning, Target.effectController.GetFloatingAttributes().Gongji.Value, 8)));
                    BulletStaticScaleChangeSystem.RegistObject(b,0,8,5,p);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }

    public class Skill4 : SkillCommonFor14_18
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "密集锁敌弹幕";
            Tag = "多体、压制、连击";
            Description = "为范围内所有敌人生成预警圈，0.8秒后分5波快速发射密集弹幕，每波精准锁定单个敌人，对多个目标形成持续压制";
            TimeNeeded = 2.2f;
            cd = 7f;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            foreach (var i in Target.GetEnemyInRange()) WarningCircle.Warn(i.transform, 1.5f, 1);
            // 每个敌人生成5颗子弹，快速连续发射
            for (int i = 0; i < 5; i++)
            {
                AddEvent(0.8f+i * 0.15f, (d) =>
                {
                    foreach (var i in Target.GetEnemyInRange())
                    {
                        var b = GetBullet(7);
                        b.Init(0.9f);
                        BulletAimSystem.RegistObject(b,0.7f,2f,d.Target.transform.position,20,i.transform.position);
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
            cd = 36f;
        }
    }
}