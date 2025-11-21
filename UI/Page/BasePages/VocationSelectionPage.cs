using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Variety.Base;

public class VocationSelectionPage : BasePage
{
    public Transform ColumnLayoutRoot;
    private List<SkillLayoutColumn>Columns=new List<SkillLayoutColumn>();
    [Header("Detail")]
    [SerializeField]private GameObject Detail;
    [SerializeField]private Image SkillIcon;
    [SerializeField]private TMP_Text SkillName;
    [SerializeField] private TMP_Text SkillTag;
    [SerializeField] private TMP_Text SkillDes;
    public override void Init()
    {
        var skillCollection= Tool.VarietyManager.GetAllSkills();
        var names = Tool.VarietyManager.GetSkillPackageName();
        var dess = Tool.VarietyManager.GetSkillPackageDes();
        for (int i = 0; i < skillCollection.Count; i++)
        {
            var c=Instantiate(Tool.PrefabManager.SkillLayoutUnit, ColumnLayoutRoot).GetComponent<SkillLayoutColumn>();
            c.Init(OnSkillUnitClicked, skillCollection[i], names[i], dess[i]);
            c.SelectedIcon.SetActive(i == PlayerInfo.Vocation);
            Columns.Add(c);
        }
    }
    public void SelectColumn(int cid)
    {
        for (int i = 0; i < Columns.Count; i++)
        {
            SkillLayoutColumn c = Columns[i];
            if(c.GetInstanceID() == cid)
            {
                c.SelectedIcon.SetActive(true);
                PlayerInfo.Vocation = i;
            }
            else
            {
                c.SelectedIcon.SetActive(false);
            }
        }
    }
    private static void OnSkillUnitClicked(SkillBase skill)
    {
        var page = Tool.PageManager.VocationSelectionPage;
        page.Detail.SetActive(true);
        page.SkillIcon.sprite = skill.sprite;
        page.SkillName.text = skill.Name;
        page.SkillTag.text = skill.Tag;
        page.SkillDes.text = skill.Description;
    }
    public override void Enter()
    {
        Detail.SetActive(false);
    }
    public void Back()
    {
        Tool.PageManager.TurnPage(PageManager.PageType.Home);
    }
}