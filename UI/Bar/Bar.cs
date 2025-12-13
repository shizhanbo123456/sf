using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class Bar : MonoBehaviour
    {
        [Header("Grapic")]
        [SerializeField] private Transform Grapic;
        [SerializeField]private Text Num;

        private float parentWidth=0;
        private float scaleStart = 0;


        public void SetValue(int value,int maxValue)
        {
            if (parentWidth < 1f) parentWidth = Grapic.parent.GetComponent<RectTransform>().rect.width;
            Num.text = value.ToString() + "/" + maxValue.ToString();
            float percent = (float)value / maxValue;
            Grapic.localPosition = new Vector3(Mathf.Lerp(-parentWidth,0,percent),0,0);
        }
        public void SetScale(float factor)
        {
            if (parentWidth < 1f) parentWidth = Grapic.parent.GetComponent<RectTransform>().rect.width;
            if (scaleStart < 0.01f) scaleStart = Grapic.parent.localScale.x;
            Grapic.parent.localScale = new Vector3(factor * scaleStart, Grapic.parent.localScale.y, 1);
            Grapic.parent.localPosition = new Vector3(parentWidth*Grapic.parent.localScale.x*0.5f, 0, 0);
        }
        public void SetColor(Color c)
        {
            Grapic.GetChild(0).GetComponent<Image>().color = c;
        }
    }
    public class BarController
    {
        public Bar bar;
        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        public BarController(Bar bar)
        {
            this.bar=bar;
        }
        public void SetValue(int value, int maxValue)
        {
            if (bar != null) bar.SetValue(value, maxValue);
        }
        public void SetScale(float factor)
        {
            if(bar != null) bar.SetScale(factor);
        }
        public void SetColor(Color c)
        {
            if( bar != null) bar.SetColor(c);
        }
    }
}