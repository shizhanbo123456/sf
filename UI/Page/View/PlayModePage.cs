using EC;
using ModeTree;
using SF.UI.Bar;
using SF.UI.Skill;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePage : BasePage
{
    public enum BarType
    {
        Main,Float,Int,Pie
    }
    public enum SkillColumnType
    {
        NonCD,CD,Storable
    }

    public BarList BarList;
    public Transform SkillColumn;
    public Transform BossBarRoot;
    public Transform ScoreboardRoot;
    [SerializeField]private List<GameObject> HostOnlyObjects = new List<GameObject>();
    [Header("Effect")]
    [SerializeField]private Image HitEffect;
    [SerializeField]private CanvasGroup KilledSign;
    private float KilledSignalTimeLeft;
    private float FlickTime = -1f;
    [Space]
    [Header("Settings")]
    [SerializeField] private Text TimePassed;
    [SerializeField] private Text ModeName;
    [SerializeField] private Transform SettingsRoot;
    [SerializeField] private Transform SettingsShow;
    [SerializeField] private Transform SettingsHide;
    private bool SettingsOn;
    [Space]
    public Settlement settlement;


    public override void RegistEvent()
    {
        Tool.UIEventCenter.RegistEvent<DoFlickEvent>(e =>
        {
            Tool.PageManager.PlayModePage.DoFlick(e.time);
        });
        Tool.UIEventCenter.RegistEvent<ShowKilledSignalEvent>(e =>
        {
            Tool.PageManager.PlayModePage.ShowKilledSignal();
        });
    }
    public override void Enter()
    {
        foreach (var i in HostOnlyObjects) i.SetActive(FightController.localPlayerId == 0);
        ModeName.text = ModeManifest.ModeName(Tool.FightController.ModeList[0]-'0');
        KilledSignalTimeLeft = 4;
        KilledSign.alpha = 0;
        SettingsOn = false;
        SettingsRoot.position = SettingsHide.position;
    }
    public override void Exit()
    {
        for(int i = BossBarRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(BossBarRoot.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        int timecount = (int)Tool.FightController.FightTimeCount;
        TimePassed.text = timecount / 60 + ":" + timecount % 60;
        if (KilledSignalTimeLeft < 3)
        {
            KilledSignalTimeLeft += Time.deltaTime;
            if (KilledSignalTimeLeft < 1) KilledSign.alpha = KilledSignalTimeLeft;
            else if(KilledSignalTimeLeft<2)KilledSign.alpha = 1;
            else KilledSign.alpha = 3-KilledSignalTimeLeft;
        }
        if (FlickTime > 0)
        {
            FlickTime -= Time.deltaTime;
            if (FlickTime > 0)
            {
                Tool.PageManager.PlayModePage.HitEffect.color = new Color(1, 1, 1, FlickTime / 2);
            }
            else
            {
                Tool.PageManager.PlayModePage.HitEffect.color = new Color(1, 1, 1, 0);
            }
        }
        if (SettingsOn)
        {
            SettingsRoot.position = SettingsShow.position * 0.02f + SettingsRoot.position * 0.98f;
        }
        else
        {
            SettingsRoot.position = SettingsHide.position * 0.02f + SettingsRoot.position * 0.98f;
        }
    }
    public void ShowSettings(bool show)
    {
        SettingsOn= show;
    }
    private void ShowKilledSignal()
    {
        KilledSignalTimeLeft = 0;
    }
    private void DoFlick(float time)
    {
        FlickTime = Mathf.Max(FlickTime, time);
    }
    public void BackToPrepare()
    {
        Tool.NetworkCorrespondent.BackToPrepare();
    }
    public void ExitGame()
    {
        EnsInstance.Corr.ShutDown();
    }



    public Bar_Base CreateBar(BarType type)
    {
        GameObject obj = Instantiate(Tool.PrefabManager.Bars[(int)type], BarList.transform);
        Bar_Base bar = obj.GetComponent<Bar_Base>();
        BarList.Bars.Add(bar);
        BarList.LayoutBars();
        return bar;
    }
    public Skill_Base CreateSkillColumn(SkillColumnType type, Sprite sprite)
    {
        GameObject obj = Instantiate(Tool.PrefabManager.SkillColumns[(int)type], SkillColumn);
        var _base = obj.GetComponent<Skill_Base>();
        _base.Init(PlayerSkillController.Keys[SkillColumn.childCount - 1], sprite);
        return _base;
    }
    public BossBar CreateBossBar()
    {
        GameObject obj = Instantiate(Tool.PrefabManager.BossBar, BossBarRoot);
        var bar = obj.GetComponent<BossBar>();
        return bar;
    }
}
namespace EC
{
    public class DoFlickEvent : UIEvent
    {
        public float time;
        public DoFlickEvent(float time)
        {
            this.time = time;
        }
    }
    public class ShowKilledSignalEvent : UIEvent
    {
        
    }
}