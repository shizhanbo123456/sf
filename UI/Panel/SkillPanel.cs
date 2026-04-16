using LevelCreator;
using SF.UI.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    private List<SkillColumn> Columns=new();
    [SerializeField] private List<Transform> PCRoots = new();
    [SerializeField] private List<Transform> MobileRoots = new();
    public List<SkillColumn> CreateSkillColumns(ushort[] ids)
    {
        foreach (ushort id in ids)
        {
            var s = Tool.LevelCreatorManager.GetSkillInfo(id);
            GameObject obj = Instantiate(Tool.PrefabManager.SkillColumn);
            var _base = obj.GetComponent<SkillColumn>();
            Columns.Add(_base);
            _base.SetSprite(Tool.SpriteManager.GetSprite(s.icon));
            _base.SetAvailableTime(1);
        }
        Replace();
        if (Tool.TargetPlatform.PC == Tool.Instance.Platform)
        {
            for (int index = 0; index < Columns.Count; index++)
            {
                SkillColumn i = Columns[index];
                i.SetKey(LevelCreator.TargetTemplate.PlayerSkillController.Keys[index]);
            }
        }
        return Columns;
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
        if (Tool.TargetPlatform.PC == Tool.Instance.Platform)
        {
            return PCRoots[index];
        }
        else
        {
            return MobileRoots[index];
        }
    }
}