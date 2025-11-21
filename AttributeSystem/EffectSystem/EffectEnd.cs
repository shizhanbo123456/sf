using System;
using UnityEngine;

namespace AttributeSystem.Effect
{
    public class EffectEnd
    {
        private float endTime = float.MaxValue;
        private Func<bool> endEvent;
        public bool End
        {
            get
            {
                if (Time.time > endTime) return true;
                if (endEvent != null && endEvent.Invoke()) return true;
                return false;
            }
        }
        public EffectEnd(float time)
        {
            endTime = time + Time.time;
        }
        public EffectEnd(Func<bool> func)
        {
            endEvent = func;
        }
    }
}