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
    public GameObject TransitionPage;

    private Dictionary<PageType, GameObject> PageMap;
    private Dictionary<PageType, bool> IfDynamic = new()
    {
        {PageType.Home,false },
        {PageType.Prepare,false },
        {PageType.PlayMode,true },
        {PageType.Transition,false },
    };
    [Space]
    public Canvas DynamicCanvas;
    public Canvas StaticCanvas;
    public Text Version;

    public enum PageType
    {
        Home, Prepare, PlayMode,Transition,
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
            {PageType.Transition,TransitionPage },
        };
        foreach(var i in PageMap.Values)i.SetActive(false);
    }
    private void Start()
    {
        var t1 = HomeController.Instance;
        var t2 = PrepareController.Instance;
        var t3 = PlayModeController.Instance;
        var t4 = TransitionController.Instance;

        PageActive(PageType.Home, true);
    }
    public Dictionary<PageType,BasePage>Pages=new Dictionary<PageType,BasePage>();
    public void PageActive(PageType type,bool active)
    {
        if (!Pages.ContainsKey(type))
        {
            CreatePage(type);
        }
        Pages[type].gameObject.SetActive(active);
    }
    public void PageRepaint(PageType type)
    {
        if (!Pages.ContainsKey(type))
        {
            CreatePage(type);
        }
        Pages[type].Repaint();
    }
    private void CreatePage(PageType type)
    {
        var p = Instantiate(PageMap[type], IfDynamic[type]?DynamicCanvas.transform:StaticCanvas.transform);
        Pages.Add(type, p.GetComponent<BasePage>());
    }
}
