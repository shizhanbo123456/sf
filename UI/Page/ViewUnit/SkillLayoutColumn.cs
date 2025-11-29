using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Variety.Base;

public class SkillLayoutColumn : MonoBehaviour
{
    public List<SkillPreviewUnit>Units = new List<SkillPreviewUnit>();
    public GameObject SelectedIcon;
    [SerializeField]private Text ColumnName;
    [SerializeField]private Text ColumnDes;

    public void Init(Action<SkillBase> onUnitClicked,List<SkillBase> skill,string columnname,string columndes)
    {
        ColumnName.text= columnname;
        ColumnDes.text= columndes;
        for(int i=0;i<6;i++)
        {
            Units[i].OnClicked = onUnitClicked;
            Units[i].skill = skill[i];
            Units[i].skillIcon.sprite = Tool.SpriteManager.GetSprite(skill[i].sprite);
        }
        SelectedIcon.SetActive(false);
    }
    public void OnClicked()
    {
        Tool.PageManager.VocationSelectionPage.SelectColumn(GetInstanceID());
    }
}
