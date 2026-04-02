using LevelCreator.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class PlayerSkillController : TargetSkillController
    {
        public static List<KeyCode> Keys = new List<KeyCode>() { KeyCode.J, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.L, KeyCode.H };
        public override void CreateSkillController(short index)
        {
            var column = PlayModeController.Instance.CreateSkillColumn(index);
            var controller = SkillExecuter.CreateSkillController(index, target);
            controller.LoadSkillColumn(column);
            Skills.Add(controller);
        }
        public override void PreUpdate()
        {
            for (int i = 0; i < Keys.Count; i++)
                if (Tool.SubInput.CanUseSkill(i))
                    UseSkillByIndex(i);
        }
    }
}