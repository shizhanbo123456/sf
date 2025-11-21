using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class Bar_Main : Bar_Base
    {
        public Text Num;
        public Text NumShade;
        public float SubMin = 0;
        public float SubMax = 100;
        public Text SubNum;
        public Text SubNumShade;
        [Space]
        [Header("Grapic")]
        public Vector2 LocalXRange = new Vector2(-1147, 0);
        public Transform ShengmingGrapic;
        public Transform HujiaGrapic;
        public override void Init()
        {

        }
        public override void SetValue(float value)
        {
            Num.text = ((int)value).ToString() + "/" + ((int)Max).ToString();
            NumShade.text = Num.text;
            value = Mathf.Clamp(value, Min, Max);
            float percent = (value - Min) / (Max - Min);
            ShengmingGrapic.localPosition = new Vector3(Mathf.Lerp(LocalXRange.x, LocalXRange.y, percent), 0, 0);
        }
        public void SetSubValue(float value)
        {
            SubNum.text = ((int)value).ToString();
            SubNumShade.text = SubNum.text;
            value = Mathf.Clamp(value, SubMin, SubMax);
            float percent = (value - SubMin) / (SubMax - SubMin);
            HujiaGrapic.localPosition = new Vector3(Mathf.Lerp(LocalXRange.x, LocalXRange.y, percent), 0, 0);
        }
    }
}