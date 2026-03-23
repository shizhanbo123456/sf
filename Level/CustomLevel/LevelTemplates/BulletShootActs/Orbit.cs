using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=10000-10799,a=id个位,b=id十位,c=id百位,半径=a*2+2,旋转速度(角度/秒)=b*15+15,初始偏移=c*45
    public class Orbit : ShootActBase
    {
        public override ushort minId => 10000;
        public override ushort maxId => 10799;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int _a = id % 10;
            id /= 10;
            int _b = id % 10;
            id /= 10;
            int _c= id % 10;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletOrbitSystem.RegistObject(b, info.radius, info.lifeTime,_a*2+2,_b*15+15,_c*45);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}