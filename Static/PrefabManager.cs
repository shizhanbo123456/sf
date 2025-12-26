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
    public GameObject SkillColumn;
    [Space]
    public GameObject RoomInfoUnit;
    [Header("Level")]
    public List<Level> Levels = new List<Level>();
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    public TargetHeader TargetHeader;
    [Header("Target")]
    public EnsBehaviourCollection TargetCollection;
    public List<TargetGraphic> GraphicCollection= new List<TargetGraphic>();
}
