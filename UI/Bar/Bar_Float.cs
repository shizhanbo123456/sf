using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class Bar_Float : Bar_Base
    {
        [Space]
        [Header("Grapic")]
        public Vector2 LocalXRange = new Vector2(-1147, 0);
        public Transform Grapic;
        public Text Num;
        public Text NumShade;
        public override void Init()
        {

        }
        public override void SetValue(float value)
        {
            Num.text = value.ToString() + "/" + Max.ToString();
            NumShade.text = Num.text;
            float percent = (value - Min) / (Max - Min);
            Grapic.localPosition = new Vector3(Mathf.Lerp(LocalXRange.x, LocalXRange.y, percent), 0, 0);
        }
    }
}