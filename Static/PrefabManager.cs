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
    public GameObject Bar_Main;
    public GameObject Bar_Float;
    public GameObject Bar_Int;
    public GameObject Bar_Pie;
    public GameObject BossBar;
    public List<GameObject> Bars => new List<GameObject>() { Bar_Main, Bar_Float, Bar_Int, Bar_Pie };
    [Space]
    public GameObject Skill_NonCD;
    public GameObject Skill_CD;
    public GameObject Skill_Storable;
    public List<GameObject> SkillColumns => new List<GameObject>() { Skill_NonCD, Skill_CD, Skill_Storable };
    [Space]
    public GameObject ClientLayoutUnit;
    public GameObject SkillLayoutUnit;
    public GameObject RoomInfoUnit;
    [Header("Bullet")]
    public GameObject BulletWarningRect;
    public GameObject BulletWarningCircle;
    public List<GameObject> BulletList = new List<GameObject>();
    [Header("Level")]
    public SceneController.LevelType LevelType;
    public Level Home;
    public Level Prepare;
    public Level Battle;
    public Level Guard;
    public List<Level> LevelPVE = new List<Level>();
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    public EnsBehaviourCollection NetPlayerCollection;
    [Header("Target")]
    public EnsBehaviourCollection OreCollection;
    public EnsBehaviourCollection LanternCollection;
    public List<EnsBehaviourCollection>MonsterCollections=new List<EnsBehaviourCollection>();
}
