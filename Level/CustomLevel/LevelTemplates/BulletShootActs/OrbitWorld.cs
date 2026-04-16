using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class OrbitWorld : ShootActBase
    {
        public static ModSpace4 mod = new(10, 10, 40,2);//8000
        public override ushort minId => 22500;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2, out var v3, out var v4);

            Execute(shooter, info, v1 + 1, v2 * 30, v3 * 9,v4==0);
        }
        public void Execute(Target shooter, BulletInfo info, float orbitRadius, float degreePerSec, float angleOffset,bool self)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletOrbitWorldSystem.RegistObject(b, info.radius, info.lifeTime,orbitRadius, degreePerSec,angleOffset,
                (!self && t) ? t.transform.position : shooter.transform.position);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}