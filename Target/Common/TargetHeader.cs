using AttributeSystem.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 用于管理角色头顶的标识，血条/Buff栏/名字<br/>无需初始化，只有TargetGraphic可访问
    /// </summary>
    public class TargetHeader : MonoBehaviour
    {
        [SerializeField] private Transform Bar;
        [SerializeField] private GameObject BarObject;
        [Space]
        [SerializeField] private Transform EffectLayoutRoot;
        [SerializeField] private Transform EffectUnitTemplate;//挂载了EffectRender
        private List<Image> EffectUnits = new List<Image>();
        [Space]
        [SerializeField] private GameObject NameObject;
        [SerializeField] private Text Recog;
        [SerializeField] private Image Back;
        private void Awake()
        {
            EffectUnitTemplate.gameObject.SetActive(false);
        }
        public void SetBarActive(bool active)
        {
            BarObject.SetActive(active);
            EffectLayoutRoot.gameObject.SetActive(active);
        }
        public void SetBarValue(float value)
        {
            Bar.transform.localPosition = new Vector3(Mathf.Lerp(-5, 0, value), 0, 0);
            Bar.transform.localScale = new Vector3(Mathf.Lerp(0, 10, value), 0.4f, 0);
        }
        public void ShowEffects(List<EffectType> effects)
        {
            foreach (var unit in EffectUnits) unit.gameObject.SetActive(false);

            Image effectUnit;
            for (int i = 0; i < effects.Count; i++)
            {
                if (i >= EffectUnits.Count)
                {
                    Transform newUnit = Instantiate(EffectUnitTemplate.gameObject, EffectLayoutRoot).transform;
                    effectUnit = newUnit.GetComponent<Image>();
                    EffectUnits.Add(effectUnit);
                }
                else
                {
                    effectUnit = EffectUnits[i];
                }
                effectUnit.gameObject.SetActive(true);
                effectUnit.sprite = Tool.SpriteManager.EffectIcons[(int)effects[i]];
            }
        }
        public void SetNameActive(bool active)
        {
            NameObject.SetActive(active);
        }
        public void SetNameText(string value)
        {
            Recog.text = value;
            GUIStyle style = new GUIStyle()
            {
                font = Recog.font,
                fontSize = Recog.fontSize,
                fontStyle = Recog.fontStyle
            };
            Texture2D tex = new Texture2D(1024, 16);
            Vector2 size = style.CalcSize(new GUIContent(Recog.text));
            Destroy(tex);
            Back.transform.localScale = new Vector3((size.x + 30f) / 12000f, 0.008f, 1);
        }
        public void SetNameColor(Color value)
        {
            Recog.color = value;
        }
    }
}