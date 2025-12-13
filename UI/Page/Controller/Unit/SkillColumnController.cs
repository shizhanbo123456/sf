using UnityEngine;

namespace SF.UI.Skill
{
    public class SkillColumnController
    {
        public SkillColumn column;
        public SkillColumnController(SkillColumn column)
        {
            this.column = column;
        }
        public void SetAvailableTime(float time)
        {
            if (column != null) column.SetAvailableTime(time);
        }
        public void SetLabelActive(bool active)
        {
            if (column != null) column.SetLabelActive(active);
        }
    }
}