using SF.UI.Bar;
using SF.UI.Skill;
using System.Collections.Generic;

public class PlayModeController : Singleton<PlayModeController>
{
    public string modeName;
    public string modeDescription;
    private PlayModePage _page;
    public PlayModePage page
    {
        get
        {
            if (_page==null&&Tool.PageManager.Pages.TryGetValue(PageManager.PageType.PlayMode, out var value))
            {
                if (value == null) return null;
                _page= value.gameObject.activeSelf ? value as PlayModePage : null;
            }
            return _page;
        }
    }
    public void Repaint()
    {
        Tool.PageManager.PageRepaint(PageManager.PageType.PlayMode);
    }
    public PlayModeController()
    {
        Tool.FightController.OnModeNameChanged += s => { modeName = s;Repaint(); };
        Tool.FightController.OnDescriptionChanged += s => { modeDescription = s;Repaint(); };
    }

    public void ShowScoreboard(string[] horizontalHeaders, string[]verticalHeaders)
    {
        var p = page;
        if (!p) return;
        p.Scoreboard.ShowPanel(verticalHeaders, horizontalHeaders);
    }
    public void HideScoreboard()
    {
        var p = page;
        if (!p) return;
        p.Scoreboard.HidePanel();
    }
    public void SetScoreboardText(int x, int y, string data)
    {
        var p = page;
        if (!p) return;
        p.Scoreboard.SetText(x, y, data);
    }
    public Bar CreateBar()
    {
        var p = page;
        if (p == null) return null;
        return page.BarPanel.CreateBar();
    }
    public void DestroyBar(Bar bar)
    {
        if(bar == null) return;
        var p = page;
        if (p == null) return;
        p.BarPanel.DestroyBar(bar);
    }
    public List<SkillColumn> CreateSkillColumns(short[] index)
    {
        var p = page;
        if (p == null) return null;
        return p.CreateSkillColumns(index);
    }
    public void DestroyAllSkillColumns()
    {
        var p = page;
        if (p == null) return;
        p.DestroyAllSkillColumns();
    }
    public BossBar CreateBossBar()
    {
        var p = page;
        if (p == null) return null;
        return p.BarPanel.CreateBossBar();
    }
    public void DestroyBossBar(BossBar bar)
    {
        if (bar == null) return;
        var p = page;
        if (p == null) return;
        p.BarPanel.DestroyBossBar(bar);
    }

    public void ShowSelection(string label, string[] message)
    {
        var p = page;
        if (p == null) return;
        p.SelectionPanel.ShowPanel(OnSelect,label,message);
    }
    public void HideSelection()
    {
        var p = page;
        if (p == null) return;
        p.SelectionPanel.HidePanel();
    }
    public void OnSelect(int x)
    {
        Tool.NetworkCorrespondent.SelectRpc(x);
    }


    public void ShowKilledSignal()
    {
        var p = page;
        if (p == null) return;
        p.EffectPanel.ShowKilledSign();
    }
    public void ShowHitEffect()
    {
        var p = page;
        if (p == null) return;
        p.EffectPanel.OnHit();
    }

    public void ShowTitle(string title)
    {
        var p = page;
        if (p == null) return;
        p.EffectPanel.ShowTitle(title);
    }
    public void ShowSubtitle(string subtitle)
    {

        var p = page;
        if (p == null) return;
        p.EffectPanel.ShowSubTitle(subtitle);
    }


    public void Settle(int killscore,int timescore,int challengescore)
    {
        var p = page;
        if (p == null) return;
        p.settlement.Settle(killscore,timescore,challengescore);
    }
    public void BackToPrepare()
    {
        Tool.NetworkCorrespondent.BackToPrepare();
    }
    public void ExitGame()
    {
        EnsInstance.Corr.ShutDown();
    }
}