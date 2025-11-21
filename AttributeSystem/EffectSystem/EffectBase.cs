using Utils;

namespace AttributeSystem.Effect
{
    public abstract class EffectBase
    {
        public EffectEnd End;
        protected Target receiver;
        private float updateInterval;
        private ReachTime reachTime = new ReachTime();
        private int hash;
        public EffectBase(Target receiver,int hash, float lastTime, float updateInterval = -1f)
        {
            this.receiver = receiver;
            End = new EffectEnd(lastTime);
            this.updateInterval = updateInterval;
            reachTime.ReachAfter(updateInterval);
            this.hash= hash;
        }
        public EffectBase(Target receiver,int hash, EffectEnd end, float updateInterval = -1f)
        {
            this.receiver = receiver;
            End = end;
            this.updateInterval = updateInterval;
            reachTime.ReachAfter(updateInterval);
            this.hash = hash;
        }
        public void Update()
        {
            if (updateInterval < 0) return;
            if (End.End) return;
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
        public abstract Effects GetEffectType();
        public abstract bool Positive();
        public int GetCustomHash()
        {
            return hash;
        }
    }
}