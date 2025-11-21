using UnityEngine;

namespace AttributeSystem.Attributes
{
    [CreateAssetMenu(menuName = "Attributes/TargetAttributes")]
    public class TargetAttributes : ScriptableObject
    {
        public ExponentalGrowth Shengming;
        public ExponentalGrowth Gongji;
        public ExponentalGrowth Fangyu;
        public LinerGrowth Mingzhong;
        public LinerGrowth Shanbi;
        public LinerGrowth Baoji;
        public LinerGrowth Renxing;
        public LinerGrowth Jiashang;
        public LinerGrowth Jianshang;
        [Space]
        public int Kangjitui;
        public float Jixing;
        public float Tengkong;
        public int Liantiao;

        public DynamicAttributes GetDynamicAttributes(int level)
        {
            var d = new DynamicAttributes();
            d.Shengming.Value = (int)Shengming.GetValue(level);
            d.Gongji.Value = (int)Gongji.GetValue(level);
            d.Fangyu.Value = (int)Fangyu.GetValue(level);
            d.Mingzhong.Value = (int)Mingzhong.GetValue(level);
            d.Shanbi.Value = (int)Shanbi.GetValue(level);
            d.Baoji.Value = (int)Baoji.GetValue(level);
            d.Renxing.Value = (int)Renxing.GetValue(level);
            d.Jiashang.Value = Jiashang.GetValue(level);
            d.Jianshang.Value = Jianshang.GetValue(level);
            d.Kangjitui.Value = Kangjitui;
            d.Jixing.Value = Jixing;
            d.Tengkong.Value = Tengkong;
            d.Liantiao.Value = Liantiao;
            return d;
        }
    }
}