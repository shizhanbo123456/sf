using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Skill
{
    /// <summary>
    /// 整数部分表示可用次数，小数部分表示冷却比例，范围[0,~]
    /// </summary>
    public class Skill_Storable : Skill_Base
    {
        [SerializeField] private Text StoredTime;
        /// <summary>
        /// 整数部分表示可用次数，小数部分表示冷却比例，范围[0,~]
        /// </summary>
        public override void SetAvailableTime(float time)
        {
            if (time < 0) Debug.LogWarning("错误的可用次数" + time);
            if (time < 0.01f)
            {
                PieShade.fillAmount = 1;
                StoredTime.text = "0";
                return;
            }
            float t = 1 - time % 1;
            if (t > 0.999f) t = 0;
            PieShade.fillAmount = t;
            StoredTime.text = ((int)time).ToString();
        }
    }
}