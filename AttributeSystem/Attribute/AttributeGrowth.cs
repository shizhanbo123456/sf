using System;

namespace AttributeSystem.Attributes
{
    public abstract class AttributeGrowth
    {
        public abstract float GetValue(int level);
    }
    [Serializable]
    public class ExponentalGrowth : AttributeGrowth
    {
        public float baseValue;
        public float expGrowth = 1;
        public override float GetValue(int level)
        {
            return baseValue * MathF.Pow(level,expGrowth);
        }
    }
    [Serializable]
    public class LinerGrowth : AttributeGrowth
    {
        public float baseValue;
        public float growthValue;
        public override float GetValue(int level)
        {
            return baseValue +growthValue* level;
        }
    }
}