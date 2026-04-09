using LevelCreator.TargetTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    [Space]
    public List<GameObject> Tiles = new();
    //public GameObject SolidLandGraphic;//Tiles[15]
    public GameObject LevitatingPlatform;
    public GameObject BrokenPlatform;
    public GameObject Wind;
    public GameObject Spike;
    public GameObject Trampoline;

    public GameObject TileCollider;
    public GameObject PlatformCollider;

    public ObjectPool<GameObject> LevitatingPlatformPool = new(
        () => Instantiate(Tool.PrefabManager.LevitatingPlatform),
        t => t.SetActive(true),
        t => t.SetActive(false));
    public ObjectPool<GameObject> BrokenPlatformPool = new(
        () => Instantiate(Tool.PrefabManager.BrokenPlatform),
        t => t.SetActive(true),
        t => t.SetActive(false));
    public ObjectPool<GameObject> WindPool = new(
        () => Instantiate(Tool.PrefabManager.Wind),
        t => t.SetActive(true),
        t => t.SetActive(false));
    public ObjectPool<GameObject> SpikePool = new(
        () => Instantiate(Tool.PrefabManager.Spike),
        t => t.SetActive(true),
        t => t.SetActive(false));
    public ObjectPool<GameObject> TrampolinePool = new(
        () => Instantiate(Tool.PrefabManager.Trampoline),
        t => t.SetActive(true),
        t => t.SetActive(false));

    public ObjectPool<GameObject> TileColliderPool = new(
        () => Instantiate(Tool.PrefabManager.TileCollider),
        t => t.SetActive(true),
        t => t.SetActive(false));
    public ObjectPool<GameObject> PlatformColliderPool = new(
        () => Instantiate(Tool.PrefabManager.PlatformCollider),
        t => t.SetActive(true),
        t => t.SetActive(false));
    [Header("Player")]
    public GameObject UnnetPlayer;
    public EnsBehaviourCollection NonSkillPlayerCollection;
    public TargetHeader TargetHeader;
    [Header("Target")]
    public EnsBehaviourCollection TargetCollection;
    public GameObject TargetMinimap;
    public List<TargetGraphic> GraphicCollection= new List<TargetGraphic>();
}
