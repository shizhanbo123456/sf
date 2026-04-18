using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    public static class TargetAttributes
    {
        private static Dictionary<int,int>Factor=new();
        public static GameTimeAttributes GetGameTimeAttributes(int level, float healthScale = 1f)
        {
            int factor = GetFactor(level);
            var d=new GameTimeAttributes();
            d.Shengming.Value = (int)(factor * 0.1f*healthScale);
            d.Gongji.Value = (int)(factor * 0.01f);
            d.Fangyu.Value = (int)(factor * 0.01f);
            d.Mingzhong.Value = (int)(factor * 0.01f);
            d.Shanbi.Value = (int)(factor * 0.01f);
            d.Baoji.Value = (int)(factor * 0.01f);
            d.Renxing.Value = (int)(factor * 0.01f);
            d.Kangjitui.Value = 0;
            d.Jixing.Value = 5f;
            d.Tengkong.Value = 5f;
            d.Liantiao.Value = 2;
            return d;
        }
        public static int GetFactor(int level)
        {
            if (level <= 0) level = 1;
            if(level>100) level=100;
            if (Factor.TryGetValue(level, out var value))
                return value;

            const float baseLevel = 30f;
            const float baseValue = 100000f;
            const double doublingInterval = 12.0;

            double exponent = (level - baseLevel) / doublingInterval;
            float factor = baseValue * (float)Mathf.Pow(2f, (float)exponent);

            int f = ((int)factor / 1000) * 1000;

            Factor.Add(level, f);
            return f;
        }
    }
}