using AttributeSystem.Attributes;
using UnityEngine.Pool;
using Utils;

namespace AttributeSystem.Effect
{
    public abstract class EffectBase
    {
        private static ObjectPool<ReachTime> pool = new(()=>new ReachTime());
        public ReachTime end { get; private set; }
        protected Target receiver;
        private float updateInterval;
        private ReachTime reachTime;
        private int hash;
        public EffectBase(Target receiver,int hash, float lastTime, float updateInterval = -1f)
        {
            end=pool.Get();
            end.ReachAfter(lastTime);
            this.receiver = receiver;
            this.updateInterval = updateInterval;
            reachTime=pool.Get();
            reachTime.ReachAfter(updateInterval);
            this.hash= hash;
        }
        protected GameTimeAttributes GetBaseAttributes()
        {
            return receiver.BaseAttributes;
        }
        protected GameTimeAttributes GetFloatingAttributes()
        {
            return receiver.FloatingAttributes;
        }
        public void Update()
        {
            if (updateInterval < 0) return;
            if (reachTime.Reached)
            {
                Repeat();
                reachTime.ReachAfter(updateInterval);
            }
        }
        public virtual void OnEntry()
        {

        }
        public virtual void OnExit()
        {

        }
        public virtual void Repeat()
        {

        }
        public abstract EffectType GetEffectType();
        public abstract bool Positive();
        public int GetCustomHash()
        {
            return hash;
        }
        ~EffectBase()
        {
            pool.Release(end);
            pool.Release(reachTime);
            end = null;
            reachTime= null;
        }
    }
}