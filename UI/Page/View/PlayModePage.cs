using SF.UI.Bar;
using SF.UI.Skill;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePage : BasePage
{
    private static PlayModeController controller=>PlayModeController.Instance;
    public Transform BarList;
    public Transform SkillColumn;
    public Transform BossBarRoot;
    public Scoreboard Scoreboard;
    [SerializeField]private List<GameObject> HostOnlyObjects = new List<GameObject>();
    [Header("Effect")]
    [SerializeField]private Image HitEffect;
    [SerializeField]private CanvasGroup KilledSign;
    private float KilledSignalTimeLeft;
    private float FlickTime = -1f;
    private Color FlickColor;
    [Space]
    [Header("Settings")]
    [SerializeField] private CanvasGroup Settings;
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
            SkillColumn.GetChild(i).GetComponent<SkillColumn>().SetKey(PlayerSkillController.Keys[i]);
        }
    }
    private void Awake()
    {
        foreach (var i in SkillInfos) i.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        foreach (var i in HostOnlyObjects) i.SetActive(FightController.localPlayerId == 0);
        ModeName.text = CustomLevel.ModePath;
        KilledSignalTimeLeft = 4;
        KilledSign.alpha = 0;
        SettingsOn = false;
        Settings.alpha = 0;
    }
    private void Update()
    {
        int timecount = (int)CustomLevel.FightTime;
        TimePassed.text = timecount / 60 + ":" + timecount % 60;
        if (KilledSignalTimeLeft < 3)
        {
            KilledSignalTimeLeft += Time.deltaTime;
            if (KilledSignalTimeLeft < 1) KilledSign.alpha = KilledSignalTimeLeft;
            else if(KilledSignalTimeLeft<2)KilledSign.alpha = 1;
            else KilledSign.alpha = 3-KilledSignalTimeLeft;
            KilledSign.gameObject.SetActive(KilledSign.alpha>0.01f);
        }
        if (FlickTime > 0)
        {
            FlickTime -= Time.deltaTime;
            if (FlickTime > 0)
            {
                HitEffect.color = new Color(FlickColor.r, FlickColor.g, FlickColor.b, FlickTime);
            }
            else
            {
                HitEffect.color = new Color(1, 1, 1, 0);
            }
        }
        Settings.alpha += Time.deltaTime * (SettingsOn ? 2 : -2);
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
        var s = VarietyManager.GetSkill(index);
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
        Settings.gameObject.SetActive(!SettingsOn);
    }
    public void BackToPrepare()=>controller.BackToPrepare();
    public void ExitGame()=>controller.ExitGame();

    public void ShowKilledSignal()
    {
        KilledSignalTimeLeft = 0;
    }
    public void DoFlick(float time, Color color)
    {
        FlickTime = Mathf.Max(FlickTime, time);
        FlickColor= color;
    }
}