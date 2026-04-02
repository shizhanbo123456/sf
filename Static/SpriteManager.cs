using AttributeSystem.Effect;
using LevelCreator.TargetTemplate;
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
    [SerializeField] private EffectType effects;
    public List<Sprite> EffectIcons;
    [Space]
    [Header("Colors")]
    public List<Color> CampColors = new List<Color>();
    public List<Color> BulletColorsCommon = new List<Color>();
    public List<Color> BulletColorsSpecial = new List<Color>();
    public enum ColorType
    {
        Name,BulletCommon,BulletSpecial
    }
    public Sprite GetSprite(Vector2Int v)
    {
        switch (v.x)
        {
            case 1:return SkillPackageA[v.y];
            case 2:return SkillPackageB[v.y];
            case 3:return SkillPackageC[v.y];
        }
        return null;
    }
    public Color TargetToColor(int camp,ColorType type=ColorType.Name)
    {
        return GetByIndex(camp,type);
    }
    private Color GetByIndex(int index,ColorType t)
    {
        switch (t)
        {
            case ColorType.Name:return CampColors[index];
            case ColorType.BulletCommon:return BulletColorsCommon[index];
            case ColorType.BulletSpecial:return BulletColorsSpecial[index];
        }
        return Color.white;
    }
}
