using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level;

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
    public Level.LevelType LevelType;
    public Level Home;
    public Level Prepare;
    public Level Battle;
    public Level Guard;
    public List<Level> LevelPVE = new List<Level>();
    public Level GetLevel(LevelType type)
    {
        Level level = null;
        switch (type)
        {
            case LevelType.Home: level = Tool.PrefabManager.Home; break;
            case LevelType.Prepare: level = Tool.PrefabManager.Prepare; break;
            case LevelType.Luandou: level = Tool.PrefabManager.Battle; break;
            case LevelType.Gongfang: level = Tool.PrefabManager.Guard; break;
        }
        if (level == null)
        {
            level = Tool.PrefabManager.LevelPVE[(int)type - (int)LevelType.PVE1];
        }
        if (level == null) Debug.LogError("Î´ÕÒµ½¹Ø¿¨");
        return level;
    }
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    [Header("Target")]
    public EnsBehaviourCollection TargetCollection;
    public CustomTargetCreater.GraphicType graphicType;
    public List<TargetGraphic> GraphicCollection= new List<TargetGraphic>();
}
