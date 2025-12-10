using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private void Awake()
    {
        Tool.PrefabManager=this;
    }

    [Header("UI")]
    public GameObject BarBase;
    public GameObject BossBar;
    [Space]
    public GameObject Skill_NonCD;
    public GameObject Skill_CD;
    public GameObject Skill_Storable;
    public List<GameObject> SkillColumns => new List<GameObject>() { Skill_NonCD, Skill_CD, Skill_Storable };
    [Space]
    public GameObject SkillLayoutUnit;
    public GameObject RoomInfoUnit;
    [Header("Bullet")]
    public GameObject BulletWarningRect;
    public GameObject BulletWarningCircle;
    public List<GameObject> BulletList = new List<GameObject>();
    [Header("Level")]
    public List<Level> Levels = new List<Level>();
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    [Header("Target")]
    public EnsBehaviourCollection TargetCollection;
    public List<TargetGraphic> GraphicCollection= new List<TargetGraphic>();
}
