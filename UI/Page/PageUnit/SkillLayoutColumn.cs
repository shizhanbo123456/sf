using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Variety.Base;

public class SkillLayoutColumn : MonoBehaviour
{
    public List<SkillPreviewUnit>Units = new List<SkillPreviewUnit>();
    public GameObject SelectedIcon;
    [SerializeField]private TMP_Text ColumnName;
    [SerializeField]private TMP_Text ColumnDes;

    public void Init(Action<SkillBase> onUnitClicked,List<SkillBase> skill,string columnname,string columndes)
    {
        ColumnName.text= columnname;
        ColumnDes.text= columndes;
        for(int i=0;i<6;i++)
        {
            Units[i].OnClicked = onUnitClicked;
            Units[i].skill = skill[i];
            Units[i].skillIcon.sprite = skill[i].sprite;
        }
        SelectedIcon.SetActive(false);
    }
    public void OnClicked()
    {
        Tool.PageManager.VocationSelectionPage.SelectColumn(GetInstanceID());
    }
}
