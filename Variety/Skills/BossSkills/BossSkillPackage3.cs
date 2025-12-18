using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss3
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "八角锁敌";
            Tag = "单体、精准";
            Description = "从自身周围8个对角位置依次发射子弹，所有子弹均锁定最近敌人位置精准打击，形成全方位包夹";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for (int i = 0; i < 8; i++)
            {
                AddEvent(i* 0.3f,new TimeLineData(Target,i), (d) =>
                {
                    var t = d.Target.GetNearestEnemy();
                    if (!t) t = d.Target;
                    Vector3 startpos = d.Target.transform.position + new Vector3(Mathf.Cos(d.index * 0.785f), Mathf.Sin(d.index * 0.785f) + 1) * 4;
                    var b = GetBullet(7);
                    b.Init(0.7f);
                    BulletAimSystem.RegistObject(b,0.3f,4,startpos,10,t.transform.position);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "后撤散射";
            Tag = "范围、拉扯";
            Description = "向远离最近敌人的方向后撤，同时发射一枚跟随自身的高额伤害子弹，后撤结束后向敌人方向发射31枚密集散射子弹，实现拉扯反击";
            TimeNeeded = 0.5f;
            cd = 1f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var dv = t.transform.position - Target.transform.position;
            Target.ApplyMotion(new MotionDir(dv.normalized * -10, 1f, true, 1));
            var b = GetBullet(4);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,4f,1.5f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            AddEvent(1.2f,new TimeLineData(Target,dv), (d) =>
            {
                var angle = Dt2Degree(dv);
                for(int offset=-15;offset<=15;offset++)
                {
                    var b = GetBullet(7);
                    b.Init(1.5f);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,2f,20f,angle+offset);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "引力坍缩";
            Tag = "范围、控制";
            Description = "发射一枚从巨型逐渐收缩的引力持续伤害子弹，强力吸附周围敌人并持续造成伤害，限制敌人移动";
            TimeNeeded = 0.5f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(4);
            b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticScaleChangeSystem.RegistObject(b,20,0,3f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "纵列突袭";
            Tag = "全屏、封锁";
            Description = "自身短暂静止，生成6列纵向预警区域，1秒后每列依次发射垂直贯穿子弹，全面封锁纵向走位空间";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(12, false).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(2.2f, true, 1));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(0.2f * i, new TimeLineData(Target,i),(d) =>
                {
                    WarningRect.Warn(d.Target.transform.position + new Vector3(i * 10, 15), d.Target.transform.position + new Vector3(i * 10, -15),0.5f,1f);
                });
                AddEvent(0.2f * i + 1f, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(11);
                    b.Init(1.5f);
                    BulletFromToSystem.RegistObject(b,0.5f,0.5f,d.Target.transform.position + new Vector3(i * 10, 15), d.Target.transform.position + new Vector3(i * 10, -15));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "变速追踪导弹";
            Tag = "多体、压制、跟随";
            Description = "2.8秒内分7波向范围内所有敌人发射追踪导弹，导弹速度随敌人移动状态动态调整，持续追击目标造成伤害";
            TimeNeeded = 2.8f;
            cd = 15f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var enemies = Target.GetEnemyInRange();
            if (enemies.Count == 0) enemies.Add(Target);
            for (int i = 0; i < 7; i++)
            {
                AddEvent(i * 0.4f, (d) =>
                {
                    foreach (var enemy in enemies)
                    {
                        var b = GetBullet(7);
                        b.Init(0.8f);
                        BulletProectileAimSystem.RegistObject(b, 0.4f, 2f, d.Target.transform.position, Vector3.up * 20, enemy.transform.position, 1.2f);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "冰封领域";
            Tag = "全屏、控制";
            Description = "生成12范围超大预警圈，自身静止3秒蓄力，随后分批次发射逐渐扩大的冰冻弹幕，命中敌人附加3秒冻结效果，全面控制战场";
            TimeNeeded = 0.5f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 12, 0.6f);
            Target.ApplyMotion(new MotionStatic(3f, true, 1));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(i * 0.2f + 0.6f, (d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.2f,liftstoiclevel:2,ec: new EffectCollection(Target, (EffectType.Freeze, 0, 3f)));
                    BulletStaticScaleChangeSystem.RegistObject(b, 0f,12f,1f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}