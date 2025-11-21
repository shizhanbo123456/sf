using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    [Serializable]
    public class DynamicAttributes
    {
        public RegistableVariable<int> Shengming=new RegistableVariable<int>(0);
        public RegistableVariable<int> Gongji=new RegistableVariable<int>(0);
        public RegistableVariable<int> Fangyu= new RegistableVariable<int>(0);
        public RegistableVariable<int> Mingzhong = new RegistableVariable<int>(0);
        public RegistableVariable<int> Shanbi = new RegistableVariable<int>(0);
        public RegistableVariable<int> Baoji = new RegistableVariable<int>(0);
        public RegistableVariable<int> Renxing = new RegistableVariable<int>(0);
        public RegistableVariable<float> Jiashang=new RegistableVariable<float>(0);
        public RegistableVariable<float> Jianshang=new RegistableVariable<float>(0);
        public RegistableVariable<int> Kangjitui = new RegistableVariable<int>(0);
        public RegistableVariable<float> Jixing = new RegistableVariable<float>(0);
        public RegistableVariable<float> Tengkong = new RegistableVariable<float>(0);
        public RegistableVariable<int> Liantiao = new RegistableVariable<int>(0);
        public RegistableVariable<int> Hudun = new RegistableVariable<int>(0);



        public DynamicAttributes Clone()
        {
            var d= new DynamicAttributes();
            d.Shengming.Value = Shengming.Value;
            d.Gongji.Value = Gongji.Value;
            d.Fangyu.Value = Fangyu.Value;
            d.Mingzhong.Value = Mingzhong.Value;
            d.Shanbi.Value = Shanbi.Value;
            d.Baoji.Value = Baoji.Value;
            d.Renxing.Value = Renxing.Value;
            d.Jiashang.Value = Jiashang.Value;
            d.Jianshang.Value = Jianshang.Value;
            d.Kangjitui.Value = Kangjitui.Value;
            d.Jixing.Value = Jixing.Value;
            d.Tengkong.Value = Tengkong.Value;
            d.Liantiao.Value = Liantiao.Value;
            d.Hudun.Value = Hudun.Value;
            return d;
        }
        public void SetAllDirty()
        {
            Shengming.OnValueChanged?.Invoke(Shengming.Value);
            Gongji.OnValueChanged?.Invoke(Gongji.Value);
            Fangyu.OnValueChanged?.Invoke(Fangyu.Value);
            Mingzhong.OnValueChanged?.Invoke(Mingzhong.Value);
            Shanbi.OnValueChanged?.Invoke(Shanbi.Value);
            Baoji.OnValueChanged?.Invoke(Baoji.Value);
            Renxing.OnValueChanged?.Invoke(Renxing.Value);
            Jiashang.OnValueChanged?.Invoke(Jiashang.Value);
            Jianshang.OnValueChanged?.Invoke(Jianshang.Value);
            Kangjitui.OnValueChanged?.Invoke(Kangjitui.Value);
            Jixing.OnValueChanged?.Invoke(Jixing.Value);
            Tengkong.OnValueChanged?.Invoke(Tengkong.Value);
            Liantiao.OnValueChanged?.Invoke(Liantiao.Value);
            Hudun.OnValueChanged?.Invoke(Hudun.Value);
        }
    }
}