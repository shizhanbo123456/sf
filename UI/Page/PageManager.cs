using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public HomePage HomePage;
    public PreparePage PreparePage;
    public PlayModePage PlayModePage;
    public VocationSelectionPage VocationSelectionPage;
    [Space]
    public Canvas Canvas;
    public Text Version;
    public Image BgEffect;
    public Transition Transition;

    private BasePage PresentPage;
    public List<BasePage> Pages => new List<BasePage>()
    {
        HomePage,PreparePage,PlayModePage,VocationSelectionPage
    };
    public enum PageType
    {
        Home, Prepare, PlayMode, VocationSelection
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
        PresentPage = HomePage;
        for (int j = 0; j < Pages.Count; j++) Pages[j].gameObject.SetActive(j == 0);
        PresentPage.Enter();

        Version.text = "v"+Application.version;
    }
    public void TurnPage(PageType type)
    {
        int i = (int)type;
        PresentPageType=type;
        PresentPage.Exit();
        PresentPage.gameObject.SetActive(false);
        PresentPage = Pages[i];
        PresentPage.gameObject.SetActive(true);
        PresentPage.Enter();
    }
}
