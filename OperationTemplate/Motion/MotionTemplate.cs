using UnityEngine;
using Variety.Base;

namespace Variety.Template
{
    public class MotionStatic : MotionBase
    {
        public MotionStatic(float workTime, bool activeAdded, int stoicLevel) : base(workTime, activeAdded, stoicLevel)
        {
        }
        public override Vector2 Entry(Vector2 v)
        {
            return new Vector2();
        }
        public override Vector2 GetVelocity(Vector2 v)
        {
            return new Vector2();
        }
    }
    public class MotionDir : MotionBase
    {
        private Vector2 dir;
        public MotionDir(Vector2 velocity,float workTime, bool activeAdded, int stoicLevel) : base(workTime, activeAdded, stoicLevel)
        {
            dir = velocity;
        }
        public override Vector2 Entry(Vector2 v)
        {
            return dir;
        }
        public override Vector2 GetVelocity(Vector2 v)
        {
            return dir;
        }
    }
    public class MotionForce : MotionBase
    {
        private Vector2 dir;
        public MotionForce(Vector2 accleration, float workTime, bool activeAdded, int stoicLevel) : base(workTime, activeAdded, stoicLevel)
        {
            dir = accleration;
        }

        public override Vector2 GetVelocity(Vector2 v)
        {
            return v+Time.deltaTime*dir;
        }
    }
    public class MotionVelocityLerp : MotionBase
    {
        private Vector2 start;
        private Vector2 end;
        public MotionVelocityLerp(Vector2 vStart,Vector2 vEnd,float workTime, bool activeAdded, int stoicLevel) : base(workTime, activeAdded, stoicLevel)
        {
            start = vStart;
            end = vEnd;
        }
        public override Vector2 Entry(Vector2 v)
        {
            return start;
        }
        public override Vector2 GetVelocity(Vector2 v)
        {
            return Vector2.LerpUnclamped(start,end,SpawnTime/WorkTime);
        }
    }
    public class MotionVelocityChange : MotionBase
    {
        private Vector2 newV;
        public MotionVelocityChange(Vector2 vector, bool activeAdded, int stoicLevel) : base(0, activeAdded, stoicLevel)
        {
            newV = vector;
        }
        public override Vector2 Entry(Vector2 v)
        {
            return newV;
        }
        public override Vector2 GetVelocity(Vector2 v)
        {
            return v;
        }
    }
}