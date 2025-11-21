using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SF.UI.Skill
{
    /// <summary>
    /// 可用次数只可为0/1
    /// </summary>
    public class Skill_NonCD : Skill_Base
    {
        /// <summary>
        /// 只可为0/1
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            if (time < -0.01f || time > 1.01f || (time > 0.01f && time < 0.99f)) Debug.LogWarning("错误的可用次数" + time);
            if (time > 0.5f) PieShade.fillAmount = 0;
            else PieShade.fillAmount = 1;
        }
    }
}