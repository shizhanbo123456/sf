using SF.UI.Bar;
using SF.UI.Skill;

public class PlayModeController : Singleton<PlayModeController>
{
    public PlayModePage page
    {
        get
        {
            if(Tool.PageManager.Pages.TryGetValue(PageManager.PageType.PlayMode, out var value))return value.gameObject.activeSelf?value as PlayModePage:null;
            return null;
        }
    }
    public void Repaint()
    {
        Tool.PageManager.PageRepaint(PageManager.PageType.PlayMode);
    }

    public void SetScoreboardActive(bool active)
    {
        var p = page;
        if (!p) return;
        p.Scoreboard.gameObject.SetActive(active);
    }
    public void SetScoreboardText(int x, int y, string data)
    {
        var p = page;
        if (!p) return;
        p.Scoreboard.SetText(x, y, data);
    }
    public BarController CreateBar()
    {
        var p = page;
        var b= p != null?p.CreateBar():null;
        return new BarController(b);
    }
    public void DestroyBar(BarController bar)
    {
        if(bar == null) return;
        var p = page;
        if (p != null && bar.bar != null) p.DestroyBar(bar.bar);
    }
    public SkillColumnController CreateSkillColumn(int index)
    {
        var p = page;
        var c = p == null ? null : p.CreateSkillColumn(index);
        return new SkillColumnController(c);
    }
    public void DestroySkillColumn(SkillColumnController column)
    {
        if (column == null) return;
        var p = page;
        if (p!=null&&column.column!=null)p.DestroySkillColumn(column.column);
    }
    public BossBarController CreateBossBar()
    {
        var p = page;
        var b = p != null ? p.CreateBossBar() : null;
        return new BossBarController(b);
    }
    public void DestroyBossBar(BossBarController bar)
    {
        if (bar == null) return;
        var p = page;
        if (p != null && bar.bar != null)p.DestroyBossBar(bar.bar);
    }

    public void ShowKilledSignal()
    {
        var p = page;
        if (p == null) return;
        p.ShowKilledSignal();
    }
    public void DoFlick(float time,UnityEngine.Color color)
    {
        var p = page;
        if (p == null) return;
        p.DoFlick(time,color);
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