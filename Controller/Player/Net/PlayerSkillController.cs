using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class PlayerSkillController : TargetSkillController
{
    public static List<KeyCode> Keys = new List<KeyCode>() { KeyCode.J,KeyCode.U,KeyCode.I,KeyCode.O,KeyCode.L,KeyCode.H};
    public override void PreUpdate()
    {
        //此脚本如果不是本地会disable

        for (int i = 0; i < Keys.Count; i++)
            if (Tool.SubInput.CanUseSkill(i))
                UseSkill(i);
    }
}
