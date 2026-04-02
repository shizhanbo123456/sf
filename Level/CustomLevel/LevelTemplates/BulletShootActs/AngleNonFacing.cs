using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=300-399,speedFactor=tid/20,angleFactor=tid%20,speed=speedFactor*4+4,angle=angleFactor*18
    public class AngleNonFacing : ShootActBase
    {
        public override short minId => 300;
        public override short maxId => 399;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            int speedFactor = trueid / 20;
            int angleFactor = trueid % 20;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletAngleNonFacingSystem.RegistObject(b, info.radius, info.lifeTime,speedFactor*4+4,angleFactor*18);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    //id=500-599,speedFactor=tid/20,angleFactor=tid%20,speed=speedFactor*4+4,angle=angleFactor*18
    public class AngleNonFacing2 : ShootActBase
    {
        public override short minId => 500;
        public override short maxId => 599;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            int speedFactor = trueid / 20;
            int angleFactor = trueid % 20;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            float offset = 0;
            if(t)offset=BulletSystemCommon.VectorToAngle(t.transform.position - shooter.transform.position);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletAngleNonFacingSystem.RegistObject(b, info.radius, info.lifeTime, speedFactor * 4 + 4, angleFactor * 18+offset);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}