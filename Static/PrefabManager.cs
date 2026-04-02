using LevelCreator.TargetTemplate;
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
    public GameObject HomeLevel;
    public GameObject PrepareLevel;
    public GameObject FightLevel;
    public List<GameObject> LevitatingPlatform = new();
    public VelocityInArea VelocityInArea;
    public List<GameObject> Tiles = new();
    public Collider2D TileCollider = new();
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    public TargetHeader TargetHeader;
    [Header("Target")]
    public EnsBehaviourCollection TargetCollection;
    public GameObject TargetMinimap;
    public List<TargetGraphic> GraphicCollection= new List<TargetGraphic>();
}
