using System.Collections.Generic;
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
    [SerializeField]private Text SkillName;
    [SerializeField] private Text SkillTag;
    [SerializeField] private Text SkillDes;
    public override void Init()
    {
        for (int i = 0; i < VoctionSelectionViewModel.Instance.Skills.Count; i++)
        {
            var c=Instantiate(Tool.PrefabManager.SkillLayoutUnit, ColumnLayoutRoot).GetComponent<SkillLayoutColumn>();
            c.Init(OnSkillUnitClicked, VoctionSelectionViewModel.Instance.Skills[i],
                VoctionSelectionViewModel.Instance.SkillPackageNames[i],
                VoctionSelectionViewModel.Instance.SkillDes[i]);
            c.SelectedIcon.SetActive(i == PlayerInfo.Vocation);
            Columns.Add(c);
        }
    }
    private void OnEnable()
    {
        Detail.SetActive(false);
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
        page.SkillIcon.sprite = Tool.SpriteManager.GetSprite(skill.sprite);
        page.SkillName.text = skill.Name;
        page.SkillTag.text = skill.Tag;
        page.SkillDes.text = skill.Description;
    }
    public void Back()
    {
        Tool.PageManager.TurnPage(PageManager.PageType.Home);
    }
}