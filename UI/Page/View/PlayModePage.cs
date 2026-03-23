using SF.UI.Bar;
using SF.UI.Skill;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePage : BasePage
{
    private static PlayModeController controller=>PlayModeController.Instance;
    [SerializeField] private Transform BarList;
    [SerializeField] private Transform SkillColumn;
    [SerializeField] private Transform BossBarRoot;
    public Scoreboard Scoreboard;
    [SerializeField]private List<GameObject> HostOnlyObjects = new List<GameObject>();
    [Header("Effect")]
    [SerializeField]private Image FlickEffect;
    private float FlickEnd = -1f;
    private Color FlickColor;
    [SerializeField]private CanvasGroup KilledSign;
    private float KilledSignalTimeStart;
    [Space]
    [Header("Settings")]
    [SerializeField] private CanvasGroup Settings;
    private int lastFrameTime;
    [SerializeField] private Text TimePassed;
    [SerializeField] private Text ModeName;
    [SerializeField] private Text ModeDes;
    [SerializeField]private List<Transform> SkillInfos=new List<Transform>();
    private bool SettingsOn;
    [Space]
    public Settlement settlement;

    public override void Repaint()
    {
        for (int i = 0; i < SkillColumn.childCount; i++)
        {
            SkillColumn.GetChild(i).GetComponent<SkillColumn>().SetKey(LevelCreator.TargetTemplate.PlayerSkillController.Keys[i]);
        }
        foreach (var i in HostOnlyObjects) i.SetActive(EnsInstance.HasAuthority);
        ModeName.text = LevelCreator.CustomLevel.ModePath;
        ModeDes.text = LevelCreator.CustomLevel.ModeDescrpition;
        Settings.gameObject.SetActive(Settings.alpha > 0.01f);
    }
    private void Awake()
    {
        foreach (var i in SkillInfos) i.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        KilledSignalTimeStart = -10;
        KilledSign.alpha = 0;
        SettingsOn = false;
        Settings.alpha = 0;
        Repaint();
    }
    private void Update()
    {
        if (SettingsOn)
        {
            int timecount = (int)LevelCreator.CustomLevel.FightTime;
            if (lastFrameTime != timecount) TimePassed.text = timecount / 60 + ":" + timecount % 60;
        }
        if (KilledSignalTimeStart < 3+Time.time)
        {
            var s = KilledSignalTimeStart - Time.time;
            if (s < 1) KilledSign.alpha = s;
            else if(s<2)KilledSign.alpha = 1;
            else KilledSign.alpha = 3-s;
            if(KilledSign.gameObject.activeSelf!=KilledSign.alpha>0.01f)
                KilledSign.gameObject.SetActive(KilledSign.alpha>0.01f);
        }
        if (FlickEnd > Time.time)
        {
            FlickEffect.color = new Color(FlickColor.r, FlickColor.g, FlickColor.b, FlickEnd);
        }
        else
        {
            if (FlickEffect.gameObject.activeSelf) FlickEffect.gameObject.SetActive(false);
        }
        Settings.alpha += Time.deltaTime * (SettingsOn ? 3 : -3);
        Settings.gameObject.SetActive(Settings.alpha>0.01f);
    }

    public Bar CreateBar()
    {
        GameObject obj = Instantiate(Tool.PrefabManager.BarBase, BarList);
        Bar bar = obj.GetComponent<Bar>();
        return bar;
    }
    public void DestroyBar(Bar bar)
    {
        if (bar == null) return;
        Destroy(bar.gameObject);
    }
    public SkillColumn CreateSkillColumn(int index)
    {
        var s = Tool.LevelCreatorManager.GetSkill(index);
        GameObject obj = Instantiate(Tool.PrefabManager.SkillColumn, SkillColumn);
        var _base = obj.GetComponent<SkillColumn>();
        _base.SetSprite(Tool.SpriteManager.GetSprite(s.sprite));
        for(int i = 0; i < SkillInfos.Count; i++)
        {
            if (!SkillInfos[i].gameObject.activeSelf)
            {
                SkillInfos[i].gameObject.SetActive(true);
                SkillInfos[i].GetChild(0).GetComponent<Text>().text=s.Name;
                SkillInfos[i].GetChild(1).GetComponent<Image>().sprite= Tool.SpriteManager.GetSprite(s.sprite);
                SkillInfos[i].GetChild(2).GetComponent<Text>().text=s.Description;
                break;
            }
        }
        Repaint();
        return _base;
    }
    public void DestroySkillColumn(SkillColumn column)
    {
        if (column == null) return;
        Destroy(column.gameObject);
        for (int i = SkillInfos.Count - 1; i >= 0; i--)
        {
            if (SkillInfos[i].gameObject.activeSelf)
            {
                SkillInfos[i].gameObject.SetActive(false);
                break;
            }
        }
        Repaint();
    }
    public BossBar CreateBossBar()
    {
        GameObject obj = Instantiate(Tool.PrefabManager.BossBar, BossBarRoot);
        var bar = obj.GetComponent<BossBar>();
        return bar;
    }
    public void DestroyBossBar(BossBar bar)
    {
        if (bar == null) return;
        Destroy(bar.gameObject);
    }


    public void ShowSettings(bool show)
    {
        SettingsOn= show;
        Repaint();
    }
    public void BackToPrepare()=>controller.BackToPrepare();
    public void ExitGame()=>controller.ExitGame();

    public void ShowKilledSignal()
    {
        KilledSignalTimeStart = Time.deltaTime;
    }
    public void DoFlick(float time, Color color)
    {
        FlickEnd = Mathf.Max(FlickEnd, time);
        FlickColor= color;
        FlickEffect.gameObject.SetActive(true);
    }
}