using AttributeSystem.Effect;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.PackageA
{
    public class Skill0 : SkillNonCD
    {
        public Skill0(Target t) : base(t, Tool.SpriteManager.SkillPackageA[0])
        {
            Name = "霰弹";
            Description = "向前发射三颗子弹，耗魔10";
            Tag = "平a";
            TimeNeeded = 0.25f;
            cost = 30;
        }
        protected override void OnUse()
        {
            for (int i = -20; i <= 20; i+=20)
            {
                var b = GetBullet(7);
                b.Init(0.5f, liftstoiclevel: 0);
                BulletAngleSystem.RegistObject(b, 0.3f, 0.5f, 10, i);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillCD
    {
        public Skill1(Target t):base(t, Tool.SpriteManager.SkillPackageA[1])
        {
            Name = "突刺";
            Description = "连续向前二段突刺并造成伤害，期间有高额闪避";
            Tag = "平a";
            TimeNeeded = 0.6f;
            cost = 0;
            CD = 4f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            Target.effectController.AddEffect(new AgileBoost(Target, Target, 100, 0.5f));
            float vx = Target.FaceRight ? 15 : -15;
            AddEvent(0.02f, (d) => { 
                d.Target.ApplyMotion(
                new MotionDir(new Vector2(vx,0),0.2f,true,1,true,true)
                );
                var b=GetBullet(7);
                b.Init(1.5f);
                BulletOrbitSystem.RegistObject(b, 1f, 0.2f, 1.5f, 450, -45);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
            AddEvent(0.3f, (d) => {
                d.Target.ApplyMotion(
                new MotionDir(new Vector2(vx, 0), 0.2f, true, 1, true, true)
                );
                var b = GetBullet(7);
                b.Init(1.5f);
                BulletOrbitSystem.RegistObject(b, 1f, 0.2f, 1.5f, -450, 45);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill2 : SkillStorable
    {
        public Skill2(Target t):base(t, Tool.SpriteManager.SkillPackageA[2])
        {
            Name = "踏地";
            Description = "向下坠落并造成爆炸，可储存3次，耗魔50";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 50;
            MaxstoreTime = 3;
            storeTime = 3;//初始储存
            CD = 10f;
        }
        protected override void OnUse()
        {
            Target.ApplyMotion(new MotionDir(Vector3.down*20,0.5f,true,1,true,true));
            AddEvent(0.5f, (d) =>
            {
                d.Target.ApplyMotion(new MotionVelocityChange(new Vector2(0,8),true,1,true,true));
                var b = GetBullet(11);
                b.Init(3.6f,liftstoiclevel:2);
                BulletStaticSystem.RegistObject(b,3f,0.2f,d.Target.transform.position);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill3 : SkillStorable
    {
        public Skill3(Target t) : base(t, Tool.SpriteManager.SkillPackageA[3])
        {
            Name = "引力弹";
            Description = "跃起，向前砸出引力弹，牵引敌人，可储存两次，耗魔50";
            Tag = "平a";
            TimeNeeded = 1.2f;
            cost = 100;
            MaxstoreTime = 2;
            storeTime = 2;//初始储存
            CD = 20f;
        }
        protected override void OnUse()
        {
            Target.ApplyMotion(new MotionDir(new Vector2(0, 10), 0.3f, true, 1, true, true));
            AddEvent(0.3f, (d) =>
            {
                var b = GetBullet(5);
                b.Init(0.1f,hitback:(b,t)=>Bullet.FigureAttractForce(b,t));
                BulletAngleSystem.RegistObject(b,2f,1f,10f,-15f);
                BulletDamageTimeSystem.Regist(b);
                b.Shoot();
            });
        }
    }
    public class Skill4 : SkillNonCD
    {
        public Skill4(Target t) : base(t, Tool.SpriteManager.SkillPackageA[4])
        {
            Name = "机动扫射";
            Description = "后撤，并向前释放多发子弹，耗魔140";
            Tag = "平a";
            TimeNeeded = 1f;
            cost = 140;
        }
        protected override void OnUse()
        {
            float vx = Target.FaceRight ? 15 : -15;
            Target.ApplyMotion(new MotionDir(new Vector2(-vx, 0), 0.3f, true, 1, true, true));
            for(int i = 0; i < 5; i++)
            {
                AddEvent(0.07f*i, new TimeLineData(Target,i),(d) =>
                {
                    var front = new Vector3(d.Target.FaceRight ? 1 : -1, 0);
                    var b = GetBullet(16);
                    b.Init(1.2f);
                    BulletDirAwaitSystem.RegistObject(b, 0.4f, 1.6f, 0.6f, d.Target.transform.position - front * 2 + new Vector3(0, 1 - 0.5f * d.index),15,front);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
            AddEvent(0.3f, (d) =>
            {
                d.Target.ApplyMotion(new MotionStatic(0.25f, true, 1, true, true));
            });
        }
    }
    public class Skill5 : SkillCD
    {
        public Skill5(Target t) : base(t, Tool.SpriteManager.SkillPackageA[5])
        {
            Name = "逃脱引力";
            Description = "立即向上飞起，5s内大幅提升跳跃高度";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 0;
            CD = 20f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            Target.ApplyMotion(new MotionDir(new Vector2(0, 15), 0.2f, true, 1, true, true));
            Target.effectController.AddEffect(new JumpBoost(Target, Target, 8, 5));
        }
    }
}