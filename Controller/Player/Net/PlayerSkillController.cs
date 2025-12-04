using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : TargetSkillController
{
    public static List<KeyCode> Keys = new List<KeyCode>() { KeyCode.J,KeyCode.U,KeyCode.I,KeyCode.O,KeyCode.L,KeyCode.H};
    public override void CreateSkillColumn(Target data, int index)
    {
        Skills.Add(VarietyManager.GetSkill(index).CreateSkillColumn(data, true));
    }
    public override void PreUpdate()
    {
        for (int i = 0; i < Keys.Count; i++)
            if (Tool.SubInput.CanUseSkill(i))
                UseSkillBuffer(i);
    }
}
