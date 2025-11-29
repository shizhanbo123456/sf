using AttributeSystem.Effect;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;
using Variety.Template;

namespace Variety.Skill.PackageC
{
    public class Skill0 : SkillNonCD
    {
        public Skill0() : base()
        {
            Name = "上下开弓";
            Description = "召唤两发子弹射向前方，耗魔15";
            Tag = "平a";
            TimeNeeded = 0.3f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            Target.ApplyMotion(new MotionStatic(0.2f, true, 1, true, true));
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1,0);
            for(int i = 1; i >= -1; i -= 2)
            {
                var b = GetBullet(7);
                b.Init(0.8f,liftstoiclevel:0);
                BulletFromToSystem.RegistObject(b, 0.5f, 0.6f, Target.transform.position + Vector3.up*i, Target.transform.position + Vector3.down*i + front * 10);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill1 : SkillStorable
    {
        public Skill1() : base()
        {
            Name = "后撤步";
            Description = "迅速向后位移，期间中幅提升减伤";
            Tag = "平a";
            TimeNeeded = 0.5f;
            MaxstoreTime = 2;
            CD = 10f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            Target.ApplyMotion(new MotionDir(front*-20,0.25f,true,1,true,true));
            Target.effectController.AddEffect(new ArmorFortity(Target.ObjectId, Target, 50, 0.5f));
        }
    }
    public class Skill2 : SkillCD
    {
        public Skill2() : base()
        {
            Name = "挥符";
            Description = "向前发射三颗子弹，耗魔100";
            Tag = "平a";
            TimeNeeded = 0.5f;
            CD = 2f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            for (int i = 1; i >= -1; i--)
            {
                var b = GetBullet(14);
                b.Init(1.6f);
                BulletProectileAimSystem.RegistObject(b, 0.8f, 1.5f, Target.transform.position,new Vector3(0,i*3), pos + front * 10, 1f);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
    public class Skill3 : SkillCD
    {
        public Skill3() : base()
        {
            Name = "守卫者";
            Description = "召唤一圈子弹围绕自身，耗魔75";
            Tag = "平a";
            TimeNeeded = 0.2f;
            CD = 20f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? new Vector3(1, 0) : new Vector3(-1, 0);
            for (int i = 0; i < 4; i++)
            {
                var b = GetBullet(7);
                b.Init(0.8f);
                BulletOrbitSystem.RegistObject(b,0.4f,10f,2f,360,i*90);
                BulletDamageTimeSystem.Regist(b,0.5f);
                b.Shoot();
            }
        }
    }
    public class Skill4 : SkillNonCD
    {
        private static readonly List<float> offset = new List<float>()
        {
            4.1f,
            5.0f,
            4.5f,
            4.2f,
            6.4f,
            6.8f,
            5.4f,
            5.9f,
            5.4f,
            7.2f,
            5.7f,
            7.5f,
            4.0f,
            5.2f,
            6.8f,
            7.9f,
            7.3f,
            4.8f,
            5.2f,
            5.0f,
            7.7f,
            4.6f,
            6.2f,
            5.1f,
            5.6f,
            4.2f,
            4.1f,
            6.7f,
            5.5f,
            6.2f,
            6.8f,
            7.2f,
            4.3f,
            5.3f,
            6.9f,
            7.4f,
            6.7f,
            4.4f,
            5.4f,
            6.2f,
        };
        public Skill4() : base()
        {
            Name = "子弹风暴";
            Description = "前方地面不断喷发出子弹，耗魔150";
            Tag = "平a";
            TimeNeeded = 0.6f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var front = Target.FaceRight ? 1:-1;
            for (int i = 0; i < offset.Count; i++)
            {
                int j = i;
                AddEvent(j * 0.05f, new TimeLineData(Target,front,pos),(d) =>
                {
                    Vector3 start = new Vector3(d.index,0) + d.pos * offset[j] + Vector3.down * 2; 
                    var b = GetBullet(7);
                    b.Init(0.4f);
                    BulletFromToSystem.RegistObject(b, 0.3f, 0.5f, start, start + Vector3.up * 10);
                    BulletDamageOnceSystem.Regist(b);
                    b.Shoot();
                });
            }
        }
    }
    public class Skill5 : SkillCD
    {
        public Skill5() : base()
        {
            Name = "诅咒";
            Description = "向周围10m内的敌人施加诅咒";
            Tag = "平a";
            TimeNeeded = 2f;
            CD = 25f;
        }
        protected override void OnUse(Target Target, Vector3 pos, bool faceright)
        {
            var b = GetBullet(7);
            b.Init(0.8f);
            BulletStaticScaleChangeSystem.RegistObject(b,0,10,0.5f);
            //BulletDamageTimeSystem.Regist(b, 0.5f);
            b.Shoot();
            foreach (var i in Target.GetEnemyInRange(10, false))
            {
                i.ApplyEffect(new BadLuck(Target.ObjectId, i, 20, 10f));
            }
        }
    }
}