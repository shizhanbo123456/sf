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
    public List<Color> BulletColors1 = new List<Color>();
    public List<Color> BulletColors2 = new List<Color>();
    public List<Color> BulletColors3 = new List<Color>();
    public enum ColorType
    {
        Name,Bullet1,Bullet2,Bullet3
    }
    public Color TargetToColor(Target t,ColorType type=ColorType.Name)
    {
        if (t is PlayerData p) return GetByIndex(p.Camp,type);
        return Color.white;
    }
    private Color GetByIndex(int index,ColorType t)
    {
        switch (t)
        {
            case ColorType.Name:return CampColors[index];
            case ColorType.Bullet1:return BulletColors1[index];
            case ColorType.Bullet2:return BulletColors2[index];
            case ColorType.Bullet3:return BulletColors3[index];
        }
        return Color.white;
    }
}
