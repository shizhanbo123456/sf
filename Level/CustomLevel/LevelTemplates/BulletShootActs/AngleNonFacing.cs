using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=300-399,speedFactor=tid/20,angleFactor=tid%20,speed=speedFactor*4+4,angle=angleFactor*18
    public class AngleNonFacing : ShootActBase
    {
        public override ushort minId => 300;
        public override ushort maxId => 399;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            int speedFactor = trueid / 20;
            int angleFactor = trueid % 20;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletAngleNonFacingSystem.RegistObject(b, info.radius, info.lifeTime,speedFactor*4+4,angleFactor*18);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}