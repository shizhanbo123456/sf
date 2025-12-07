using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public HomePage HomePage;
    public PreparePage PreparePage;
    public PlayModePage PlayModePage;
    [Space]
    public Canvas Canvas;
    public Text Version;
    public Image BgEffect;
    public Transition Transition;

    private BasePage PresentPage;
    public List<BasePage> Pages => new List<BasePage>()
    {
        HomePage,PreparePage,PlayModePage
    };
    public enum PageType
    {
        Home, Prepare, PlayMode
    }
    public PageType PresentPageType;
    private void Awake()
    {
        Tool.PageManager=this;
    }
    private void Start()
    {
        foreach (var i in Pages)
        {
            i.Init();
            i.RegistEvent();
        }
        for (int j = 0; j < Pages.Count; j++) Pages[j].gameObject.SetActive(false);
        TurnPage(PageType.Home);

        Version.text = "v"+Application.version;
    }
    public void TurnPage(PageType type)
    {
        PresentPageType=type;
        if (PresentPage != null)
        {
            PresentPage.Exit();
            PresentPage.gameObject.SetActive(false);
        }
        PresentPage = Pages[(int)type];
        PresentPage.gameObject.SetActive(true);
        PresentPage.Enter();
    }
}
