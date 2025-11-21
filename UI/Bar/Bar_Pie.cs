using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class Bar_Pie : Bar_Base
    {
        public Image Grapic;
        public Text Num;
        public Text NumShade;
        public override void Init()
        {

        }
        public override void SetValue(float value)
        {
            Grapic.fillAmount = (value - Min) / (Max - Min);
            Num.text = value.ToString();
            NumShade.text = value.ToString();
        }
    }
}