using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class Static : ShootActBase
    {
        public static ModSpace2 mod = new(45,45);//2025
        public override ushort minId => 35500;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2);

            Execute(shooter, info, v1*2.5f-55, v2*2.5f-55);
        }
        public void Execute(Target shooter, BulletInfo info, float offsetX, float offsetY)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletStaticSystem.RegistObject(b, info.radius, info.lifeTime, shooter.transform.position + new UnityEngine.Vector3(offsetX,offsetY));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
    public class Static2 : ShootActBase
    {
        public static ModSpace2 mod = new(45, 45);//2025
        public override ushort minId => 38000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2);

            Execute(shooter, info, v1 * 2.5f - 55, v2 * 2.5f - 55);
        }
        public void Execute(Target shooter, BulletInfo info, float offsetX, float offsetY)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            UnityEngine.Vector3 pos = t ? t.transform.position : shooter.transform.position;
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletStaticSystem.RegistObject(b, info.radius, info.lifeTime, pos + new UnityEngine.Vector3(offsetX, offsetY));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}