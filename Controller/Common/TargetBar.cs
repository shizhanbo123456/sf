using AttributeSystem.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBar : MonoBehaviour
{
    [SerializeField] private Transform Bar;
    [Space]
    [SerializeField] private Transform EffectLayoutRoot;
    [SerializeField] private Transform EffectUnitTemplate;//π“‘ÿ¡ÀEffectRender
    private List<SpriteRenderer> EffectUnits = new List<SpriteRenderer>();
    private void Awake()
    {
        EffectUnitTemplate.gameObject.SetActive(false);
    }
    public void SetNum(float value)
    {
        Bar.transform.localPosition = new Vector3(Mathf.Lerp(-5, 0, value), 0, 0);
        Bar.transform.localScale = new Vector3(Mathf.Lerp(0, 10, value), 0.4f, 0);
    }
    public void ShowEffects(List<EffectType> effects)
    {
        foreach (var unit in EffectUnits) unit.gameObject.SetActive(false);

        for (int i = 0; i < effects.Count; i++)
        {
            SpriteRenderer effectUnit;
            if (i >= EffectUnits.Count)
            {
                Transform newUnit = Instantiate(EffectUnitTemplate.gameObject, EffectLayoutRoot).transform;
                effectUnit = newUnit.GetComponent<SpriteRenderer>();
                EffectUnits.Add(effectUnit);
            }
            else
            {
                effectUnit = EffectUnits[i];
            }
            effectUnit.gameObject.SetActive(true);
            effectUnit.sprite = GetIcon(effects[i]);
        }
    }
    public Sprite GetIcon(EffectType e)
    {
        return Tool.SpriteManager.EffectIcons[(int)e];
    }
}
