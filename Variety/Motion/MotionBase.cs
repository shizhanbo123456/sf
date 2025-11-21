using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variety.Base
{
    public abstract class MotionBase
    {
        public float WorkTime = 10;
        public bool ActiveAdded;
        public int StoicLevel;
        public bool MoveLock;
        public bool SkillLock;

        public float StartTime;
        public float SpawnTime
        {
            get { return Time.time - StartTime; }
        }

        public MotionBase(float workTime, bool activeAdded, int stoicLevel,bool moveLock,bool skillLock)
        {
            WorkTime = workTime;
            ActiveAdded = activeAdded;
            StoicLevel = stoicLevel;
            MoveLock= moveLock;
            SkillLock= skillLock;

            StartTime = Time.time;
        }

        //传入当前速度，传出生效速度
        public abstract Vector2 GetVelocity(Vector2 v);

        public virtual Vector2 Entry(Vector2 v)
        {
            return v;
        }
        public virtual Vector2 Exit(Vector2 v)
        {
            return v;
        }
    }
}
