using AttributeSystem.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss2
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "八角精准打击";
            Tag = "单体、包夹";
            Description = "从自身周围8个均匀分布的方向依次发射子弹，所有子弹均锁定最近敌人精准飞行，形成全方位包夹打击";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for(int i = 0; i < 8; i++)
            {
                AddEvent(i * 0.3f,new TimeLineData(Target,i), (d) =>
                {
                    var t = d.Target.GetNearestEnemy(99999, false);
                    if (t == null) t = d.Target;
                    Vector3 startpos = d.Target.transform.position + new Vector3(Mathf.Cos(d.index * 0.785f), Mathf.Sin(d.index * 0.785f)) * 4;
                    var b = GetBullet(7);
                    b.Init(0.7f,liftstoiclevel:0);
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
            Name = "突进散射斩";
            Tag = "范围、突进";
            Description = "向最近敌人方向高速突进1.5秒，期间发射一枚跟随自身的高额伤害子弹，突进结束后向目标方向发射31枚密集散射子弹，实现突进衔接爆发";
            TimeNeeded = 0.5f;
            cd = 1f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t=Target.GetNearestEnemy();
            var dv = t ? t.transform.position - Target.transform.position : Target.Front;
            Target.ApplyMotion(new MotionDir(dv.normalized * 20, 1.5f, true, 1));
            var b = GetBullet(4);
            b.Init(2.5f);
            BulletFollowSystem.RegistObject(b,4f,1.5f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            AddEvent(1.2f, new TimeLineData(Target,dv),(d) =>
            {
                var angle=Dt2Degree(d.pos);
                for(int offset = -15; offset <= 15; offset++)
                {
                    var b = GetBullet(7);
                    b.Init(1.5f);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,2,20,angle+offset);
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
            Name = "烈焰领域";
            Tag = "范围、持续伤害";
            Description = "发射一枚从无到有逐渐扩大的巨型弹幕，命中敌人后附加燃烧效果，每秒造成自身攻击力50%的伤害，持续10秒";
            TimeNeeded = 0.5f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            //GetBullet(7).Init(new BulletAngle(Target, 1, 5, 0, 0.3f), new BulletDataSlight(Target, new Damage_Once(), 0.5f)).Shoot();
            var b = GetBullet(4);
            b.Init(0.2f,liftstoiclevel:0,ec: new EffectCollection(Target, (EffectType.Burning, Target.DedicatedAttributes.Gongji * 0.5f, 10f)));
            BulletStaticScaleChangeSystem.RegistObject(b,0f,15f,1f);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "双环爆破";
            Tag = "全屏、封锁";
            Description = "自身短暂静止，在周围生成内外两层共12个预警圈，1秒后所有预警圈同时触发爆炸，全面封锁周围区域";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(12,false).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 _, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(2.2f, true, 1));
            for(int i= 0; i < 6; i++)
            {
                var pos = Target.transform.position + Vector3.up*2+10*new Vector3(Mathf.Cos(3.14f/6f*i),Mathf.Sin(3.14f/6f*i));
                AddEvent(0.2f * i,new TimeLineData(Target,pos), (d) =>
                {
                    WarningCircle.Warn(d.pos, 2.5f, 1f);
                });
                AddEvent(0.2f * i+1f, new TimeLineData(Target,pos),(d) =>
                {
                    var b = GetBullet(3);
                    b.Init(1.5f);
                    BulletStaticSystem.RegistObject(b,2.5f,0.5f,d.pos);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
                pos = Target.transform.position + Vector3.up * 2 + 10 * new Vector3(Mathf.Cos(3.14f / 6f * i + 3.14f), Mathf.Sin(3.14f / 6f * i + 3.14f));
                AddEvent(0.2f * i, new TimeLineData(Target, pos), (d) =>
                {
                    WarningCircle.Warn(d.pos, 2.5f, 1f);
                });
                AddEvent(0.2f * i + 1f, new TimeLineData(Target, pos), (d) =>
                {
                    var b = GetBullet(3);
                    b.Init(1.5f);
                    BulletStaticSystem.RegistObject(b, 2.5f, 0.5f, d.pos);
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
                        b.Init(0.6f, liftstoiclevel: 0);
                        BulletAngleNonFacingSystem.RegistObject(b, 0.8f, 2, 15, angle);
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
            Name = "震荡领域";
            Tag = "全屏、控制";
            Description = "生成12范围超大预警圈，自身静止3秒蓄力，随后分批次发射逐渐扩大的震荡弹幕，命中敌人附加3秒眩晕效果，掌控战场节奏";
            TimeNeeded = 0.5f;
            cd = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 12, 0.6f);
            Target.ApplyMotion(new MotionStatic(3f, true, 1));
            for (int i = 0; i < 6; i++)
            {
                AddEvent(i * 0.2f + 0.6f,(d) =>
                {
                    var b = GetBullet(4);
                    b.Init(0.2f,liftstoiclevel:2,ec: new EffectCollection(d.Target, (EffectType.Stun, 0, 3f)));
                    BulletStaticScaleChangeSystem.RegistObject(b,0f,12f,1f);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}