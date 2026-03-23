using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=100-199,speed=tid个位&4+4,angle=tid十位*15-60
    public class Angle : ShootActBase
    {
        public override ushort minId => 100;
        public override ushort maxId => 199;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            int tidb = trueid % 10;
            trueid /= 10;
            int tida = trueid % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletAngleSystem.RegistObject(b, info.radius, info.lifeTime, tidb * 4 + 4, tida * 15f - 60f);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
        //id=200-299,speed=tid个位&4+4,angle=tid十位*15-60
        public class Angle2 : ShootActBase
        {
            public override ushort minId => 200;
            public override ushort maxId => 299;
            public override void Act(Target shooter, BulletInfo info, ushort id)
            {
                if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
                int trueid = id - minId;
                int tidb = trueid % 10;
                trueid /= 10;
                int tida = trueid % 10;
                BulletSystemCommon.CurrentShooter = shooter;
                var b = GetBullet(info.graphicType);
                var t = shooter.GetNearestEnemy();
                float offset = 0;
                if (t)
                {
                    offset=BulletSystemCommon.VectorToAngle(t.transform.position - shooter.transform.position);
                }
                b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
                BulletAngleSystem.RegistObject(b, info.radius, info.lifeTime, tidb * 4 + 4, tida * 15f - 60f+offset);
                BulletDamageOnceSystem.Regist(b);
                b.Shoot();
            }
        }
    }
}