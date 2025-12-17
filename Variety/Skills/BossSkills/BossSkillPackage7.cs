using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss7
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "天坠重击";
            Tag = "范围、爆发";
            Description = "自身先向上跃起，短暂停滞后续猛然下坠，落地时触发范围爆炸，对4范围内敌人造成高额伤害";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            WarningCircle.Warn(Target.transform.position, 4, 1);
            Target.ApplyMotion(new MotionDir(new Vector2(0, 20), 0.5f, true, 1));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.3f, true, 1));
            });
            AddEvent(0.8f, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(Vector2.down * 50, 0.2f, true, 1));
            });
            AddEvent(1, (d) =>
            {
                d.Target.ApplyMotion(new MotionDir(new Vector2(0, 10), 0.2f, true, 1));
                var b = GetBullet(11);
                b.Init(1.5f);
                BulletStaticSystem.RegistObject(b,4f,0.5f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill1 : SkillBoss
    {
        public Skill1() : base()
        {
            sprite = new Vector2Int(1, 0);
            Name = "冲刺散射";
            Tag = "范围、突进";
            Description = "向面朝方向冲刺的同时，发射5枚呈±10度、±5度、0度分布的散射子弹，覆盖10范围内敌人";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(10, true).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = new Vector3(Target.FaceRight ? 1 : -1, 0, 0);
            Target.ApplyMotion(new MotionDir(front * 10, 0.5f, true, 1));
            for(int offset=-10;offset<=10;offset+=5)
            {
                var b = GetBullet(7);
                b.Init(1.5f);
                BulletAngleSystem.RegistObject(b,0.7f,3,5,offset);
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
            Name = "三点爆破";
            Tag = "范围、封锁";
            Description = "自身向上小幅升空，同时在自身及左右5单位位置生成预警圈，0.5秒后三个位置同时触发爆炸，封锁横向移动路径";
            TimeNeeded = 0.5f;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var p = Target.transform.position;
            Target.ApplyMotion(new MotionVelocityChange(Vector2.up*10,true,1));
            WarningCircle.Warn(p, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.right*5, 2, 0.5f);
            WarningCircle.Warn(p+Vector3.left*5, 2, 0.5f);
            AddEvent(0.5f, new TimeLineData(Target,p),(d) =>
            {
                for(int i = -1; i <= 1; i++)
                {
                    var b = GetBullet(11);
                    b.Init(1.4f);
                    BulletStaticSystem.RegistObject(b,2f,0.3f,d.pos+new Vector3(i*5,0));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                }
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "引力升空";
            Tag = "近距离、控制";
            Description = "发射一枚跟随自身的引力持续伤害子弹，同时自身缓慢升空，2秒后额外发射一枚向斜下方飞行的持续伤害子弹，双重吸附近距离敌人";
            TimeNeeded = 0.5f;
            cd = 25f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(3, false).Count > 0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(5);
            b.Init(0.1f, hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
            BulletFollowSystem.RegistObject(b,3,2,Target);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionVelocityLerp(Vector2.up * 10, Vector2.up * 5, 2, true, 1));
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.1f);
                BulletDirSystem.RegistObject(b,3f,2f,10f,new Vector2(2,-1));
                BulletDamageTimeSystem.Regist(b,0.2f);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "引力坍缩";
            Tag = "范围、控制";
            Description = "自身短暂静止，发射一枚逐渐缩小的巨型引力子弹，持续吸附周围敌人并造成持续伤害，2秒后发射一枚垂直向下的持续伤害子弹收尾";
            TimeNeeded = 0.5f;
            cd = 30f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(2, true, 1));
            var b = GetBullet(5);
            b.Init(0.1f,  hitback: (b, t) => Bullet.FigureAttractForce(b, t));
            BulletStaticScaleChangeSystem.RegistObject(b,8f,3f,2f);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
            Target.ApplyMotion(new MotionVelocityLerp(Vector2.up * 10, Vector2.up * 5, 2, true, 1));
            AddEvent(2, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.1f);
                BulletDirSystem.RegistObject(b,3f,3f,8f, Vector2.down);
                BulletDamageTimeSystem.Regist(b, 0.2f);
                b.Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "圣耀增幅";
            Tag = "全屏、增益、破甲";
            Description = "5秒蓄力期间生成巨型收缩弹幕，蓄力结束后触发大范围爆炸，命中敌人附加20点破甲效果（持续30秒），同时为所有队友附加30点伤害提升和20点幸运值（均持续30秒）";
            TimeNeeded = 0.5f;
            cd = 60f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(5, true, 1));
            var b = GetBullet(5);
            b.Init(0f);
            BulletStaticScaleChangeSystem.RegistObject(b,20,0,5);
            b.Shoot();
            AddEvent(5, (d) =>
            {
                var b = GetBullet(5);
                b.Init(2,liftstoiclevel:0, ec: new EffectCollection(d.Target, (EffectType.ArmorShatter, 20, 30)));
                BulletStaticScaleChangeSystem.RegistObject(b,0f,20f,1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
                var t=Target.GetPartnerInRange(999999, false);
                foreach(var i in t)
                {
                    i.ApplyEffect(new DamageBoost(d.Target.ObjectId, i, 30, 30));
                    i.ApplyEffect(new Luck(d.Target.ObjectId, i, 20, 30));
                }
            });
        }
    }
}