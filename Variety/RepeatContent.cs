using System;

namespace Variety.Base
{
    public abstract class RepeatContent
    {
        public float dt=1;
        private static int indexSource;
        private int index;
        public RepeatContent()
        {
            dt = 1f;
            index = indexSource++;
        }
        public abstract void Repeat(Target target);
        public override int GetHashCode()
        {
            return index;
        }
    }
}