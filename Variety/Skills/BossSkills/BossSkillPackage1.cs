using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss1
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "环形弹幕阵";
            Tag = "全屏、压制";
            Description = "向360度方向均匀发射24枚子弹，每枚间隔15度，形成无死角环形弹幕，全面封锁敌人走位";
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for(int i = 0; i < 24; i++)
            {
                var b = GetBullet(15);
                b.Init(0.5f,liftstoiclevel:0);
                BulletAngleSystem.RegistObject(b,0.6f,5,10,i*15);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "精准散射";
            Tag = "单体、范围";
            Description = "锁定最近敌人方向，向目标及左右±10度、±20度范围发射5枚子弹，精准覆盖目标周围区域";
            cd = 8f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var angle=Dt2Degree(Target.GetNearestEnemy(1000, false).transform.position-Target.transform.position);
            for(int i = -2; i < 3; i++)
            {
                var b = GetBullet(12);
                b.Init(0.9f);
                BulletAngleNonFacingSystem.RegistObject(b,0.6f,2,15,angle+i*10);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill2 : SkillBoss
    {
        public Skill2() : base()
        {
            sprite = new Vector2Int(2, 0);
            Name = "冲锋突袭";
            Tag = "突进、爆发";
            Description = "当敌人进入20×3矩形区域时触发，向面朝方向高速冲锋，同时发射一枚跟随自身的高额伤害子弹，突击路径上敌人";
            cd = 10f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRect(20, 3, true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Target.Front * 20, 1, true, 2));
            var b = GetBullet(14);
            b.Init(3.2f);
            BulletFollowSystem.RegistObject(b, 2f, 2f, Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        private static List<Vector3> VelocityStart = new List<Vector3>() 
        {
            new Vector3(6,11),
            new Vector3(-6,11),
            new Vector3(11,6),
            new Vector3(-11,6),
        };
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "天坠流星爆";
            Tag = "全屏、爆发";
            Description = "自身向上跃起后向最近敌人方向横向位移，随后猛然下坠，落地时触发范围爆炸，同时向4个斜向发射抛物线子弹，形成双重打击";
            TimeNeeded = 0.5f;
            cd = 25f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Vector2.up * 30, 1, true, 1));
            AddEvent(1, (d) =>
            {
                var t=d.Target.GetNearestEnemy();
                float x=t!=null?(t.transform.position.x-d.Target.transform.position.x):0;
                d.Target.ApplyMotion(new MotionDir(new Vector2(x*2,0), 0.5f, true, 1));
            });
            AddEvent(1.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(Vector2.down * 60, 0.5f, true, 1));
            });
            AddEvent(2f, (d) =>
            {
                var b = GetBullet(11);
                b.Init(2.8f);
                BulletStaticSystem.RegistObject(b, 6f, 0.4f, d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();

                foreach (var i in VelocityStart)
                {
                    b = GetBullet(7);
                    b.Init(1.6f);
                    BulletProectileSystem.RegistObject(b, 2f, 4f, d.Target.transform.position,i);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "双侧封锁";
            Tag = "范围、封锁";
            Description = "当5范围内无敌人时触发，从自身左右两侧分批次发射20枚子弹，子弹等待1.2秒后横向穿行，封锁两侧移动路径";
            TimeNeeded = 0.5f;
            cd = 5f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(5,false).Count==0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var p = Target.transform.position;
            for(int i = 0; i < 10; i++)
            {
                AddEvent(i * 0.1f,new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(0.9f);
                    BulletDirAwaitSystem.RegistObject(b, 0.5f, 6,1.2f, p + new Vector3(5, d.index * 1.3f), 8,Vector3.right);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                    b = GetBullet(12);
                    b.Init(0.9f);
                    BulletDirAwaitSystem.RegistObject(b, 0.5f, 6, 1.2f, p + new Vector3(-5, d.index * 1.3f), 8, Vector3.left);
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
            Name = "沉默连环爆";
            Tag = "全屏、控制";
            Description = "当敌人进入30×5矩形区域时触发，先使所有敌人沉默3秒，随后分两波在横向多个位置生成预警圈并触发爆炸，连环封锁战场";
            TimeNeeded = 0.5f;
            cd = 15f;
        }
        public override bool Detect(Target target)
        {
            return target.GetEnemyInRect(30, 5).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            foreach(var i in Target.GetEnemyInRange())
            {
                i.ApplyEffect(new EffectCollection(Target.ObjectId, (EffectType.Silence, 0, 3)));
            }
            for(int i = -5; i < 5; i+=2)
            {
                WarningCircle.Warn(Target.transform.position + new Vector3(i * 5, 0, 0), 3, 1);
            }
            AddEvent(0.5f, (d) =>
            {

                for (int i = -4; i < 4; i += 2)
                {
                    WarningCircle.Warn(d.Target.transform.position + new Vector3(i * 5, 0, 0), 3, 1);
                }
            });
            AddEvent(1, (d) =>
            {
                for (int i = -5; i < 5; i += 2)
                {
                    var b = GetBullet(11);
                    b.Init(2.8f);
                    BulletStaticSystem.RegistObject(b,3,0.3f, d.Target.transform.position + new Vector3(i * 5, 0, 0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
            AddEvent(1.5f, (d) =>
            {
                for (int i = -4; i < 4; i += 2)
                {
                    var b = GetBullet(11);
                    b.Init(2.8f);
                    BulletStaticSystem.RegistObject(b, 3, 0.3f, d.Target.transform.position + new Vector3(i * 5, 0, 0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
}