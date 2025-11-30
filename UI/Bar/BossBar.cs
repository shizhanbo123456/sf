using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class BossBar : MonoBehaviour
    {
        [SerializeField] private List<Color> Colors = new List<Color>();
        [SerializeField] private Color ColorEnd;
        [Space]
        [SerializeField]private Text Name;
        [SerializeField] private Text Num;
        [SerializeField] private Text Blood;
        [Space]
        [SerializeField] private RectTransform RootWidth;
        [SerializeField] private Image GraphicUpon;
        [SerializeField] private Image GraphicBelow;
        [SerializeField] private Transform TransformUpon;
        [SerializeField] private Transform TransformTransition;
        [Space]
        [SerializeField] private float TransitionSpeed = 0.03f;


        public void Init(string name, int maxlayer, int maxValue)
        {
            Name.text = name;
            SetValue(maxValue, maxValue, maxlayer);
        }
        public void SetValue(int value,int maxValue,int maxLayer)
        {
            if (maxValue % maxLayer != 0) Debug.LogWarning("每层的值需要为整数");
            int valueEach = maxValue / maxLayer;//每层的血量数值
            float UponLayerPosRate = value==0?0:((value - 0.00001f) % valueEach / valueEach);//顶层血量位置比例
            int layerLost = (maxValue-value) / valueEach;//损失的层数
            int currentLayerCount = maxLayer-layerLost;//当前层数

            Num.text = '×'+currentLayerCount.ToString();
            Tool.stringBuilder.Clear();
            Blood.text = Tool.stringBuilder.Append(value).Append('/').Append(maxValue).ToString();

            if (currentLayerCount <= 1)
                GraphicBelow.color = ColorEnd;
            else
                GraphicBelow.color = Colors[(layerLost + 1) % Colors.Count];
            GraphicUpon.color = Colors[layerLost % Colors.Count];

            TransformUpon.localPosition = new Vector3(Mathf.Lerp(-RootWidth.rect.width,0,UponLayerPosRate),0,0);
        }
        public void Update()
        {
            TransformTransition.localPosition = new Vector3(
                Mathf.Lerp(TransformTransition.localPosition.x,TransformUpon.localPosition.x,TransitionSpeed), 0, 0);
        }
    }
}