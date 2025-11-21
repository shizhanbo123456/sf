using AttributeSystem.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private void Awake()
    {
        Tool.SpriteManager = this;
    }
    public List<Sprite>SkillPackageA=new List<Sprite>();
    public List<Sprite>SkillPackageB=new List<Sprite>();
    public List<Sprite>SkillPackageC=new List<Sprite>();
    [Space]
    [SerializeField] private Effects effects;
    public List<Sprite> EffectIcons;
    [Space]
    [Header("Colors")]
    public List<Color> CampColors = new List<Color>();
    public Color TargetToColor(Target t)
    {
        if (t is PlayerData p) return CampColors[p.Camp];
        return Color.white;
    }
}
