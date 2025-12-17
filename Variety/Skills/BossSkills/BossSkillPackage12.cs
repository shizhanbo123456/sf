using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss12
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "精准追击";
            Tag = "单体、突进";
            Description = "发射一枚跟随目标的子弹，同时自身向前小幅突进，快速打击近距离敌人";
            TimeNeeded = 0.3f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? 1 : -1;
            var b = GetBullet(7);
            b.Init(1.8f);
            BulletFollowSystem.RegistObject(b,0.4f,0.25f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 0.25f, true, 1));
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "散射突袭";
            Tag = "范围、连击";
            Description = "先向左右两侧发射散射子弹，短暂延迟后发射一枚强力追踪弹，同时自身向前突进";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(4, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            for(int a = -10; a <= 10; a += 10)
            {
                var b = GetBullet(7);
                b.Init(0.4f,liftstoiclevel:0);
                BulletAngleSystem.RegistObject(b,0.3f,1.5f,5,a);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
            AddEvent(0.5f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;

                var b = GetBullet(7);
                b.Init(2.5f);
                BulletFollowSystem.RegistObject(b,1,0.25f,Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                d.Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1));
            });
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "蓄力爆破";
            Tag = "范围、减速";
            Description = "在自身周围生成预警圈，随后连续发射3枚逐渐扩大的爆炸子弹，同时自身获得加速效果";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 3f, 1f);
            for (int i = 0; i < 3; i++)
            {
                AddEvent(1f+i * 0.3f, (d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.5f);
                    BulletStaticScaleChangeSystem.RegistObject(b,1,3,0.3f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            Target.ApplyEffect(new Speed(Target.ObjectId, Target, 3f, 12f));
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "引力冲击";
            Tag = "远程、控制";
            Description = "发射一枚具有引力效果的持续伤害子弹，短暂静止后发射强力追踪弹并向前突进";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(8, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(5);
            b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletStaticScaleChangeSystem.RegistObject(b,4,0,1f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionStatic(0.7f, true, 1));
            AddEvent(0.8f, (d) =>
            {
                var front = d.Target.FaceRight ? 1 : -1;
                var b = GetBullet(14);
                b.Init(4,liftstoiclevel:2);
                BulletFollowSystem.RegistObject(b,0.4f,0.25f,d.Target);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                Target.ApplyMotion(new MotionDir(new Vector2(front * 30, 0), 0.25f, true, 1));
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "旋绕弹幕";
            Tag = "范围、机动";
            Description = "自身向前突进的同时，连续发射4枚沿轨道旋转的弹幕，交替顺时针和逆时针旋转";
            TimeNeeded = 1f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var front = Target.FaceRight ? 1 : -1;
            Target.ApplyMotion(new MotionDir(new Vector2(front * 20, 0), 1, true, 1));
            for (int i = 0; i < 4; i++)
            {
                AddEvent(i * 0.25f,new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(14);
                    b.Init(1.2f);
                    BulletOrbitSystem.RegistObject(b,0.5f,0.25f,1.3f,(d.index % 2 == 0) ? 360 : -360, (d.index % 2 == 0) ? -135 : 135);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "星陨狂潮";
            Tag = "全屏、爆发";
            Description = "腾空跃起后锁定最近敌人位置，生成预警区域，随后360度全方位发射抛物线弹幕覆盖战场";
            TimeNeeded = 5f;
            cd = 32f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Vector2.up * 40, 0.25f, true, 1));
            var t = Target.GetNearestEnemy();
            var p = t != null ? t.transform.position : Target.transform.position;
            AddEvent(0.25f,new TimeLineData(Target,p), (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(5f, true, 1));
                WarningCircle.Warn(d.pos, 2, 0.6f);
            });
            for (int i = 0; i < 36; i++)
            {
                var angle = i * 20f * Mathf.Deg2Rad;
                AddEvent(0.9f + i * 0.1f,new TimeLineData(Target,p), (d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.5f,liftstoiclevel:0);
                    BulletProectileAimSystem.RegistObject(b,0.7f,2,d.Target.transform.position, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 20, d.pos,1.6f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}