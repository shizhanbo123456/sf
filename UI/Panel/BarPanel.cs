using SF.UI.Bar;
using UnityEngine;

public class BarPanel:MonoBehaviour
{
    [SerializeField] private Transform BarList;
    [SerializeField] private Transform BossBarRoot;
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

}