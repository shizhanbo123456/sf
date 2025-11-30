using UnityEngine;

namespace AttributeSystem.Attributes
{
    [CreateAssetMenu(menuName = "Attributes/TargetAttributes")]
    public class TargetAttributes : ScriptableObject
    {
        public float Shengming=1;
        public float Gongji=1;
        public float Fangyu=1;
        public float Mingzhong=1;
        public float Shanbi=1;
        public float Baoji=1;
        public float Renxing=1;
        [Space]
        public int Kangjitui;
        public float Jixing=5;
        public float Tengkong=5;
        public int Liantiao=2;

        public GameTimeAttributes GetDynamicAttributes(int level)
        {
            var d = GetGameTimeAttributesTemplate(level);

            d.Shengming.Value = (int)(d.Shengming.Value*Shengming);
            d.Gongji.Value = (int)(d.Gongji.Value*Gongji);
            d.Fangyu.Value = (int)(d.Fangyu.Value* Fangyu);
            d.Mingzhong.Value = (int)(d.Mingzhong.Value* Mingzhong);
            d.Shanbi.Value = (int)(d.Shanbi.Value* Shanbi);
            d.Baoji.Value = (int)(d.Baoji.Value* Baoji);
            d.Renxing.Value = (int)(d.Renxing.Value* Renxing);

            d.Kangjitui.Value = Kangjitui;
            d.Jixing.Value = Jixing;
            d.Tengkong.Value = Tengkong;
            d.Liantiao.Value = Liantiao;
            return d;
        }
        public static GameTimeAttributes GetGameTimeAttributesTemplate(int level)
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
            return d;
        }
        public static float GetFactor(int level)
        {
            const float expGrowth = 3f;
            return Mathf.Pow(level+100, expGrowth);//10800 86400 291600 691200 1350000  3200000
        }
    }
}