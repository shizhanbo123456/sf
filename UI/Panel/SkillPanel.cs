using LevelCreator;
using SF.UI.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    [NonSerialized]public List<SkillColumn> Columns=new();
    [SerializeField] private List<Transform> PCRoots = new();
    [SerializeField] private List<Transform> MobileRoots = new();
    public SkillColumn CreateSkillColumn(short index)
    {
        var s = Tool.LevelCreatorManager.GetSkillInfo(index);
        GameObject obj = Instantiate(Tool.PrefabManager.SkillColumn);
        var _base = obj.GetComponent<SkillColumn>();
        Columns.Add(_base);
        _base.SetSprite(Tool.SpriteManager.GetSprite(s.sprite));
        Replace();
        return _base;
    }
    public void DestroySkillColumn(SkillColumn column)
    {
        if (column == null) return;
        Destroy(column.gameObject);
        Replace();
    }
    public void DestroyAllSkillColumns()
    {
        foreach (var column in Columns)
        {
            if(column == null) continue;
            Destroy(column.gameObject);
        }
        Columns.Clear();
    }
    private void Replace()
    {
        Columns.RemoveAll(t => t == null);
        for(int i = 0; i < Columns.Count; i++)
        {
            Columns[i].transform.SetParent(GetRoot(i));
            Columns[i].transform.localPosition = Vector3.zero;
        }
    }
    private Transform GetRoot(int index)
    {
        if (Tool.TargetPlatform.Windows == Tool.Instance.Platform)
        {
            return PCRoots[index];
        }
        else
        {
            return MobileRoots[index];
        }
    }
}