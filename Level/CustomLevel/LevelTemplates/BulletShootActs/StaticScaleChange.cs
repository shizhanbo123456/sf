using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;

namespace LevelCreator.BulletShootTemplate
{
    //id=17011-19499,水平偏移=十位*5-25，垂直偏移=个位*5-25，尺寸参数=千位百位组成的两位数
    //初始尺寸参数=尺寸参数/5，结尾尺寸参数=尺寸参数%5，尺寸倍率=尺寸参数*0.5
    public class StaticScaleChange : ShootActBase
    {
        public override short minId => 17011;
        public override short maxId => 19499;
        public override void Act(Target shooter, BulletInfo info, short id)
        {
            if (id < minId || id > maxId) throw new System.Exception("id越界，id=" + id);
            int tid = id - minId;
            int v = tid % 10;
            tid /= 10;
            int h = tid % 10;
            tid /= 10;
            tid -= minId / 100;
            int startFactor = tid / 5;
            int endFactor = tid % 5;
            BulletSystemCommon.CurrentShooter = shooter;
            var b = GetBullet(info.graphicType);
            var t = shooter.GetNearestEnemy();
            var effectinfo = Tool.LevelCreatorManager.GetEffectInfo(info.effect);
            b.Init(info.rate, info.liftstoiclevel, new EffectCollection(shooter.ObjectId, effectinfo.effects?.ToArray()), info.hitbackForce);
            BulletStaticScaleChangeSystem.RegistObject(b, info.radius*startFactor*0.5f,info.radius*endFactor*0.5f ,info.lifeTime,
                shooter.transform.position+new UnityEngine.Vector3(h*5-25,v*5-25));
            BulletDamageOnceSystem.Regist(b);
            b.Shoot();
        }
    }
}