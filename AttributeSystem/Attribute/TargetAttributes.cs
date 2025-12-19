using UnityEngine;

namespace AttributeSystem.Attributes
{
    public class TargetAttributes
    {
        public static GameTimeAttributes GetGameTimeAttributes(int level, float healthScale = 1f)
        {
            float factor = GetFactor(level);
            var d=new GameTimeAttributes();
            d.Shengming.Value = (int)(factor * 0.1f);
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
        public static float GetFactor(int level)
        {
            const float expGrowth = 3f;
            return Mathf.Pow(level+100, expGrowth);//10800 86400 291600 691200 1350000  3200000
        }
    }
}