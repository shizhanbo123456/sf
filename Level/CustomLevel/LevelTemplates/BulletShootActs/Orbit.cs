using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Orbit : ShootActBase
    {
        public static ModSpace3 mod = new(10,10,40);//4000
        public override ushort minId => 18000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2, out var v3);

            Execute(shooter,info, v1+1, v2*30, v3*9);
        }
        public void Execute(Target shooter, BulletInfo info, float orbitRadius,float degreePerSec, float angleOffset)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletOrbitSystem.RegistObject(b, info.radius, info.lifeTime,orbitRadius,degreePerSec,angleOffset);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}