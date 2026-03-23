using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=11000-11799和12000-12799,a=id个位,b=id十位,c=id百位,半径=a*2+2,旋转速度(角度/秒)=b*15+15,初始偏移=c*45，千位=0时在自身位置，=1时在最近敌人位置
    public class OrbitWorld : ShootActBase
    {
        public override ushort minId => 0;
        public override ushort maxId => 9;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int _a = id % 10;
            id /= 10;
            int _b = id % 10;
            id /= 10;
            int _c = id % 10;
            id /= 10;
            bool self = id % 10 == 1;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletOrbitWorldSystem.RegistObject(b, info.radius, info.lifeTime, _a * 2 + 2, _b * 15 + 15, _c * 45,
                (!self&&t)?t.transform.position:shooter.transform.position);
            BulletDamageTimeSystem.Regist(b);
            b.Shoot();
        }
    }
}