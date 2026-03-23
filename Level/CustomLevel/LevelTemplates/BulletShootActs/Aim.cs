using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=0-9,speed=3+tid*6
    public class Aim:ShootActBase
    {
        public override ushort minId => 0;
        public override ushort maxId => 9;
        public override void Act(Target shooter, BulletInfo info,ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var p = t == null ? shooter.transform.position + shooter.Front : t.transform.position;
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletAimSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position, 3+trueid*6, p);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}