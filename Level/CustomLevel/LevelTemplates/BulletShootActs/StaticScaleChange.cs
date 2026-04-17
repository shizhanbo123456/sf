using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class StaticScaleChange : ShootActBase
    {
        public static ModSpace4 mod = new(9, 9,5,5);//2025
        public override ushort minId => 40500;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2, out var v3, out var v4);

            Execute(shooter, info,v3*0.5f,v4*0.5f,v1 * 2.5f - 10, v2 * 2.5f - 10);
        }
        public void Execute(Target shooter, BulletInfo info, float startRadiusFactor,float endRadiusFactor,float offsetX,float offsetY)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletStaticScaleChangeSystem.RegistObject(b, info.radius * startRadiusFactor, info.radius * endRadiusFactor, info.lifeTime,
                shooter.transform.position + new UnityEngine.Vector3(offsetX, offsetY));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}