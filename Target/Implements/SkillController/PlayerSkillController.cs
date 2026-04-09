using LevelCreator.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class PlayerSkillController : TargetSkillController
    {
        public static List<KeyCode> Keys = new List<KeyCode>() { KeyCode.J, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.L, KeyCode.H };
        public override void CreateSkillControllers(ushort[] ids)
        {
            var columns = PlayModeController.Instance.CreateSkillColumns(ids);
            for (int i = 0; i < columns.Count; i++)
            {
                SF.UI.Skill.SkillColumn column = columns[i];
                var controller = SkillExecuter.CreateSkillController(ids[i], target);
                controller.LoadSkillColumn(column);
                Skills.Add(controller);
            }
        }
        public override void PreUpdate()
        {
            for (int i = 0; i < Keys.Count; i++)
                if (Tool.SubInput.CanUseSkill(i))
                {
                    UseSkillByIndex(i);
                }
        }
    }
}