using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    public class GameTimeAttributes
    {
        public RegistableVariable<int> Shengming=RegistableVariable<int>.Get();
        public RegistableVariable<int> Gongji= RegistableVariable<int>.Get();
        public RegistableVariable<int> Fangyu= RegistableVariable<int>.Get();
        public RegistableVariable<int> Mingzhong = RegistableVariable<int>.Get();
        public RegistableVariable<int> Shanbi = RegistableVariable<int>.Get();
        public RegistableVariable<int> Baoji = RegistableVariable<int>.Get();
        public RegistableVariable<int> Renxing = RegistableVariable<int>.Get();
        public RegistableVariable<int> Jiashang= RegistableVariable<int>.Get();
        public RegistableVariable<int> Jianshang= RegistableVariable<int>.Get();
        public RegistableVariable<int> Kangjitui = RegistableVariable<int>.Get();
        public RegistableVariable<float> Jixing = RegistableVariable<float>.Get();
        public RegistableVariable<float> Tengkong = RegistableVariable<float>.Get();
        public RegistableVariable<int> Liantiao = RegistableVariable<int>.Get();

        private bool released = false;

        public GameTimeAttributes Clone()
        {
            var d= new GameTimeAttributes();
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
        }
        public void Release()
        {
            if (released) return;
            RegistableVariable<int>.Release(Shengming);
            RegistableVariable<int>.Release(Gongji);
            RegistableVariable<int>.Release(Fangyu);
            RegistableVariable<int>.Release(Mingzhong);
            RegistableVariable<int>.Release(Shanbi);
            RegistableVariable<int>.Release(Baoji);
            RegistableVariable<int>.Release(Renxing);
            RegistableVariable<int>.Release(Jiashang);
            RegistableVariable<int>.Release(Jianshang);
            RegistableVariable<int>.Release(Kangjitui);
            RegistableVariable<float>.Release(Jixing);
            RegistableVariable<float>.Release(Tengkong);
            RegistableVariable<int>.Release(Liantiao);
            released = true;
        }
    }
}