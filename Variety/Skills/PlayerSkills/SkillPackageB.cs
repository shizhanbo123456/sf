using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.PackageB
{
    public class Skill0 : SkillNonCD
    {
        public Skill0(Target t) : base(t, Tool.SpriteManager.SkillPackageB[0])
        {
            Name = "肘击";
            Description = "前方小范围造成高额伤害,并获得幸运效果3s，耗魔10";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 10;
        }
        protected override void OnUse()
        {
            Target.effectController.AddEffect(new Luck(Target, Target, 20, 3));
            var b = GetBullet(11);
            b.Init(2.2f);
            BulletStaticSystem.RegistObject(b,0.8f,0.3f, Target.transform.position + (Target.FaceRight ? new Vector3(2, 0) : new Vector3(-2, 0)));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill1 : SkillCD
    {
        public Skill1(Target t) : base(t, Tool.SpriteManager.SkillPackageB[1])
        {
            Name = "震撼";
            Description = "震飞周围敌人，可击破霸体单位，期间有超级霸体且防御大幅提升，耗魔10";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 30;
            CD = 20f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            Target.effectController.AddEffect(new DefenseBoost(Target, Target, 8f, 0.5f));
            Target.ApplyMotion(new MotionStatic(0.5f, true, 2, true, true));
            var b = GetBullet(6);
            b.Init(0.2f,liftstoiclevel:2);
            BulletStaticScaleChangeSystem.RegistObject(b,0,2,0.3f);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill2 : SkillCD
    {
        public Skill2(Target t) : base(t, Tool.SpriteManager.SkillPackageB[2])
        {
            Name = "三发箭";
            Description = "向前下方发射三发子弹，魔20";
            Tag = "平a";
            TimeNeeded = 0.2f;
            cost = 20;
            CD = 1f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            for(int i = 0; i >= -60; i -= 30)
            {
                var b = GetBullet(7);
                b.Init(1.5f);
                BulletAngleSystem.RegistObject(b,0.6f,1f,10,i);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
            Target.ApplyMotion(new MotionStatic(0.4f, true, 1, true, false));
        }
    }
    public class Skill3 : SkillCD
    {
        public Skill3(Target t) : base(t, Tool.SpriteManager.SkillPackageB[3])
        {
            Name = "力场";
            Description = "释放力场持续伤害范围内敌人，耗魔180";
            Tag = "平a";
            TimeNeeded = 0.1f;
            cost = 180;
            CD = 20f;
            storeTime = 1;//初始储存次数
        }
        protected override void OnUse()
        {
            var b = GetBullet(4);
            b.Init(0.5f,liftstoiclevel:0);
            BulletFollowSystem.RegistObject(b,3f,15f,Target);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Skill4 : SkillNonCD
    {
        List<float> offset = new List<float>()
        {
            4f,-0.3f,-3.2f,4.6f,1.5f,
            0.8f,2.2f,2.6f,-4.7f,3.9f,
            0.2f,-0.5f,5f,4.4f,-4.6f,
            -5f,-3.8f,-1.7f,-1.5f,-0.5f
        };
        public Skill4(Target t) : base(t, Tool.SpriteManager.SkillPackageB[4])
        {
            Name = "引雷";
            Description = "在周围召唤大量子弹落下，耗魔200";
            Tag = "平a";
            TimeNeeded = 1f;
            cost = 200;
        }
        protected override void OnUse()
        {
            Target.ApplyMotion(new MotionStatic(0.5f, true, 1, true, true));
            for(int i = 0; i < 20; i++)
            {
                AddEvent(i * 0.1f,new TimeLineData(Target, Target.transform.position), (d) =>
                {
                    var b = GetBullet(12);
                    b.Init(1.2f);
                    BulletFromToSystem.RegistObject(b,0.5f,1.5f, d.pos + new Vector3(offset[d.index], 5), d.pos + new Vector3(offset[d.index], -5));
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillStorable
    {
        public Skill5(Target t) : base(t, Tool.SpriteManager.SkillPackageB[5])
        {
            Name = "冲刺";
            Description = "向前冲刺一段距离，期间中幅提升闪避";
            Tag = "平a";
            TimeNeeded = 0.5f;
            cost = 0;
            MaxstoreTime = 4;
            storeTime = 4;//初始储存
            CD = 10f;
        }
        protected override void OnUse()
        {
            Target.effectController.AddEffect(new AgileBoost(Target, Target, 40, 0.5f));
            Target.ApplyMotion(new MotionDir(Target.FaceRight ? new Vector2(15, 0) : new Vector2(-15, 0), 0.5f, true, 1, true, true));
        }
    }
}