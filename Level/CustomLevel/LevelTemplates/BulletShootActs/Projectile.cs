using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=13010-13099.水平初速度=十位*4-20,垂直初速度=个位*2
    public class Projectile : ShootActBase
    {
        public override ushort minId => 13010;
        public override ushort maxId => 13099;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int vertical = id % 10;
            id /= 10;
            int horizontal = id % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletProjectileSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position,new UnityEngine.Vector3(horizontal*4-20,vertical*2));
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}