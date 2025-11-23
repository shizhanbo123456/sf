using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Bar
{
    public class BossBar : MonoBehaviour
    {
        public List<Color> Colors = new List<Color>();
        public Color ColorEnd;
        [Space]
        public Text Name;
        public Text Num;
        public Text Blood;
        public Image GraphicUpon;
        public Image GraphicBelow;
        public Transform TransformUpon;
        public Transform TransformTransition;
        private RectTransform Rect;
        private float width;
        [Space]
        public float PresentNum;
        public float TransitionNum;//0-1
        public float TransitionSpeed = 0.98f;
        [Space]
        [SerializeField] public float scalefactor = 3f;

        private int max;
        private int bloodperm;

        private bool Initialized = false;

        public void Init()
        {
            Rect = GetComponent<RectTransform>();
            width = Rect.rect.width*scalefactor;

            Initialized = true;
        }
        public void Enter(string name, int maxlayer, int maxValue)
        {
            if (!Initialized) Init();
            gameObject.SetActive(true);
            Name.text = name;
            TransformTransition.localPosition = new Vector3();

            max = maxValue;
            bloodperm = maxValue / maxlayer;
            SetLayer(maxlayer);
            SetBlood(maxValue);
        }
        public void SetBlood(int p)
        {
            Blood.text = p + "/" + max;
            SetLayer(p * 1f / bloodperm);
        }
        private void SetLayer(float layer)
        {
            if (layer < 0) layer = 0;
            string t = "×" + (1000 - (int)(1000 - layer + 0.0001f)).ToString();
            Num.text = t;
            if ((int)PresentNum != (int)layer) TransitionNum = 1;
            PresentNum = layer;
            if (PresentNum <= 1)//Below (int)(num)  Upon (int)(num+1)
            {
                GraphicBelow.color = ColorEnd;
                GraphicUpon.color = Colors[(int)(layer + 1) % Colors.Count];
            }
            else
            {
                GraphicBelow.color = Colors[(int)layer % Colors.Count];
                GraphicUpon.color = Colors[(int)(layer + 1) % Colors.Count];
            }
            TransformUpon.localPosition = new Vector3(-width + ((layer - 0.0001f) % 1) * width, 0, 0);
        }
        public void Update()
        {
            TransitionNum = TransitionNum * TransitionSpeed + (PresentNum - 0.0001f) % 1 * (1 - TransitionSpeed);
            TransformTransition.localPosition = new Vector3(-width + (TransitionNum - 0.0001f % 1) * width, 0, 0);
        }
    }
}