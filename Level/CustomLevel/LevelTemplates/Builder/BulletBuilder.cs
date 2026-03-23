namespace LevelCreator
{
    public static class BulletBuilder
    {
        public static void Create(int bulletType,float radius,float rate,int liftstoiclevel = 1, float hitbackForce=0)
        {

        }
        public static void AddEffect(EffectType type,float degree ,float time)
        {
        }
        public static void Upload()
        {

        }
    }
    public struct BulletInfo:Info
    {
        public int graphicType;
        public float radius;
        public float lifeTime;
        public float rate;
        public int liftstoiclevel;
        public float hitbackForce;
        public SingleEffect[] Effects;
        public BulletInfo(int graphicType,float radius,float lifeTime, float rate, int liftstoiclevel, float hitbackForce)
        {
            this.graphicType = graphicType;
            this.radius=radius;
            this.lifeTime=lifeTime;
            this.rate = rate;
            this.liftstoiclevel = liftstoiclevel;
            this.hitbackForce = hitbackForce;
            Effects = null;
        }
        public BulletInfo(int graphicType, int radius,float lifeTime,float rate, int liftstoiclevel, float hitbackForce, SingleEffect[] effects)
        {
            this.graphicType = graphicType;
            this.radius = radius;
            this.lifeTime = lifeTime;
            this.rate = rate;
            this.liftstoiclevel = liftstoiclevel;
            this.hitbackForce = hitbackForce;
            Effects = effects;
        }
    }
}