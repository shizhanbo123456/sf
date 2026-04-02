using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=100-199,speed=tid个位&4+4,angle=tid十位*15-60
    public class Angle : ShootActBase
    {
        public override short minId => 100;
        public override short maxId => 199;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int trueid = id - minId;
            int tidb = trueid % 10;
            trueid /= 10;
            int tida = trueid % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletAngleSystem.RegistObject(b, info.radius, info.lifeTime, tidb * 4 + 4, tida * 15f - 60f);
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}