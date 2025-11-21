using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 设置为浮点表示冷却剩余比例，范围[0,1]
    /// </summary>
    public class Skill_CD : Skill_Base
    {
        /// <summary>
        /// 设置为浮点表示冷却剩余比例，范围[0,1]
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            if (time < 0 || time > 1) Debug.LogWarning("错误的可用次数" + time);
            PieShade.fillAmount = 1 - time;
        }
    }
}