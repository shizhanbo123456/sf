using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=1111-9999,a=id个位,b=id十位,c=id百位,d=id千位，start=playerPosition+Vector2(d*5-25,c*5-25),end=playerPosition+Vector2(b*5-25,a*5-25)
    public class FromTo : ShootActBase
    {
        public override ushort minId => 1111;
        public override ushort maxId => 9999;
        public override void Act(Target shooter, BulletInfo info, ushort id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int _a = id % 10;
            id /= 10;
            int _b= id % 10;
            id /= 10;
            int _c= id % 10;
            id /= 10;
            int _d = id;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, info.Effects), info.hitbackForce);
            BulletFromToSystem.RegistObject(b, info.radius, info.lifeTime, 
                shooter.transform.position+new UnityEngine.Vector3(_d*5-25,_c*5-25),
                shooter.transform.position+new UnityEngine.Vector3(_b*5-25,_a*5-25)
                );
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}