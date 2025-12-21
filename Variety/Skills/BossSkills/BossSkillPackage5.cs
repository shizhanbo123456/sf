using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.Boss5
{
    public class Skill0 : SkillBoss
    {
        public Skill0() : base()
        {
            sprite = new Vector2Int(0, 0);
            Name = "扇形连射";
            Tag = "范围、精准";
            Description = "锁定最近敌人方向，先发射1枚主子弹，随后向左右±5度至±20度范围分批次发射9枚散射子弹，形成扇形弹幕覆盖目标区域";
            TimeNeeded = 0.5f;
            cd = 8f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy();
            var angle = t?Dt2Degree(t.transform.position - Target.transform.position):(Target.FaceRight?0:180);

            var b = GetBullet(7);
            b.Init(0.3f,liftstoiclevel: 0);
            BulletAngleNonFacingSystem.RegistObject(b,0.3f,5f,10f,angle);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
            for (int i = -4; i <=4; i++)
            {
                AddEvent(0.1f *Mathf.Abs(i),new TimeLineData(Target,i), (d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.3f,  liftstoiclevel: 0);
                    BulletAngleNonFacingSystem.RegistObject(b,0.3f,5f,10f,angle+d.index*5);
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
            Name = "五向抛物线";
            Tag = "范围、压制";
            Description = "同时发射5枚抛物线子弹，呈30度至150度均匀分布，沿重力轨迹飞行，覆盖前方大范围区域";
            TimeNeeded = 0.5f;
            cd = 10f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for (int i = 0; i < 5; i++)
            {
                float rad = (30 + i * 30)*Mathf.Deg2Rad;
                var b = GetBullet(7);
                b.Init(0.5f);
                BulletProectileSystem.RegistObject(b,1f,3f,Target.transform.position,new Vector3(Mathf.Cos(rad),Mathf.Sin(rad))*12);
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
            Name = "升空散射";
            Tag = "范围、爆发";
            Description = "自身向上跃起1秒，落地后锁定最近敌人方向，向目标及左右±10度范围发射密集散射子弹";
            TimeNeeded = 1f;
            cd = 10f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionDir(Vector2.up * 20, 1f, true, 2));
            AddEvent(1f, (d) =>
            {
                var p = d.Target.GetNearestEnemy();
                var angle= Dt2Degree(p.transform.position - Target.transform.position);
                for(int offset=-10;offset<=10;offset++)
                {
                    var b = GetBullet(7);
                    b.Init(0.5f, liftstoiclevel: 0);
                    BulletAngleSystem.RegistObject(b, 0.3f, 2f,15f, angle);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                };
            });
        }
    }
    public class Skill3 : SkillBoss
    {
        public Skill3() : base()
        {
            sprite = new Vector2Int(3, 0);
            Name = "环形轨道炮";
            Tag = "单体、控制";
            Description = "为范围内每个敌人生成预警圈，1秒后在每个敌人位置生成4枚沿轨道旋转的子弹，持续环绕打击目标";
            TimeNeeded = 1.2f;
            cd = 20f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetNearestEnemy(20f);
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var list = Target.GetEnemyInRange();
            foreach (var i in list) WarningCircle.Warn(i.transform.position, 2, 1);
            AddEvent(1f, (d) =>
            {
                foreach (var i in list)
                {
                    for(int j=0;j<4;j++)
                    {
                        var b = GetBullet(12);
                        b.Init(0.3f);
                        BulletOrbitWorldSystem.RegistObject(b,0.8f,10f,4f,90,90*j,i.transform.position);
                        BulletDamageOnceSystem.Regist(b);
                        b.Shoot();
                    }
                }
            });
        }
    }
    public class Skill4 : SkillBoss
    {
        public Skill4() : base()
        {
            sprite = new Vector2Int(4, 0);
            Name = "瞬移冲击";
            Tag = "单体、突进";
            Description = "锁定80范围内最近敌人，生成预警圈，0.6秒后瞬移至目标附近，随后在目标位置触发范围爆炸，造成高额伤害";
            TimeNeeded = 1.5f;
            cd = 3f;
        }
        public override bool Detect(Target Target)
        {
            return Target.GetEnemyInRange(8,true).Count>0;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var t = Target.GetNearestEnemy(80, true);
            if (t == null) return;
            WarningCircle.Warn(t.transform, 2.5f, 0.6f);
            Vector3 v=new Vector3();
            AddEvent(0.6f, (d) =>
            {
                v= t.transform.position;
                d.Target.ApplyMotion(new MotionDir((t.transform.position - d.Target.transform.position) * 2.5f, 0.4f, true, 1));
            });
            AddEvent(1f, (d) =>
            {
                var b = GetBullet(7);
                b.Init(2.2f);
                BulletStaticSystem.RegistObject(b,2.5f,0.3f,v);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill5 : SkillBoss
    {
        public Skill5() : base()
        {
            sprite = new Vector2Int(5, 0);
            Name = "暴雨弹幕";
            Tag = "全屏、压制";
            Description = "5秒内连续发射100枚子弹，子弹角度随发射顺序递增，形成高密度、大范围的弹幕雨，全面封锁敌人走位";
            TimeNeeded = 5;
            cd = 3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            for(int i = 0; i < 100; i++)
            {
                AddEvent(i * 0.05f, new TimeLineData(Target,i),(d) =>
                {
                    var b = GetBullet(7);
                    b.Init(0.9f);
                    BulletAngleSystem.RegistObject(b, 0.3f, 2f, 20f, 7 * d.index);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
}