using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    public class FromTo : ShootActBase
    {
        public static ModSpace4 mod = new(11,11,11,11);//14641
        public override ushort minId => 3000;
        public override ushort maxId => (ushort)(minId + mod.TotalSize - 1);
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);

            mod.Decode(id - minId, out var v1, out var v2,out var v3,out var v4);

            Execute(shooter,info,v1*4-20,v2*4-20,v3*4-20,v4*4-20);
        }
        public void Execute(Target shooter, BulletInfo info, float startX,float startY,float endX,float endY)
        {
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftStoicLevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitBackForce);
            BulletFromToSystem.RegistObject(b, info.radius, info.lifeTime,
                shooter.transform.position + new UnityEngine.Vector3(startX,startY),
                shooter.transform.position + new UnityEngine.Vector3(endX,endY)
                );
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}