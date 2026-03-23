using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class PlayerSkillController : TargetSkillController
    {
        public static List<KeyCode> Keys = new List<KeyCode>() { KeyCode.J, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.L, KeyCode.H };
        public override void CreateSkillColumn(Target data, int index)
        {
            Skills.Add(Tool.LevelCreatorManager.GetSkill(index).CreateSkillController(data, index, true));
        }
        public override void PreUpdate()
        {
            for (int i = 0; i < Keys.Count; i++)
                if (Tool.SubInput.CanUseSkill(i))
                    UseSkillBuffer(i);
        }
    }
}