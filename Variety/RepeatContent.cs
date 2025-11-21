using System;

namespace Variety.Base
{
    public abstract class RepeatContent
    {
        protected Target target;
        protected float dt;
        private float recorder;
        public RepeatContent(Target t)
        {
            target = t;
            //dt = 1f;
        }
        public void Update()
        {
            recorder += UnityEngine.Time.deltaTime;
            if (recorder > dt)
            {
                recorder -= dt;
                Repeat();
            }
        }
        protected abstract void Repeat();
    }
}