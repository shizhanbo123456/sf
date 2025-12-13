using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public GameObject HomePage;
    public GameObject PreparePage;
    public GameObject PlayModePage;

    private Dictionary<PageType, GameObject> PageMap;
    [Space]
    public Canvas Canvas;
    public Text Version;
    public Image BgEffect;
    public Transition Transition;

    public enum PageType
    {
        Home, Prepare, PlayMode
    }
    private void Awake()
    {
        Tool.PageManager = this;
        Version.text = "v" + Application.version;

        PageMap = new Dictionary<PageType, GameObject>()
        {
            {PageType.Home,HomePage },
            {PageType.Prepare,PreparePage },
            {PageType.PlayMode,PlayModePage },
        };
        foreach(var i in PageMap.Values)i.SetActive(false);
    }
    private void Start()
    {
        var t1 = HomeController.Instance;
        var t2 = PrepareController.Instance;
        var t3 = PlayModeController.Instance;

        PageActive(PageType.Home, true);
    }
    public Dictionary<PageType,BasePage>Pages=new Dictionary<PageType,BasePage>();
    public void PageActive(PageType type,bool active)
    {
        if (!Pages.ContainsKey(type))
        {
            var p=Instantiate(PageMap[type], Canvas.transform);
            Pages.Add(type,p.GetComponent<BasePage>());
        }
        Pages[type].gameObject.SetActive(active);
    }
    public void PageRepaint(PageType type)
    {
        if (!Pages.ContainsKey(type))
        {
            var p = Instantiate(PageMap[type], Canvas.transform);
            Pages.Add(type, p.GetComponent<BasePage>());
        }
        Pages[type].Repaint();
    }
}
