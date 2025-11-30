using SF.UI.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Skill
{
    /// <summary>
    /// 整数部分表示可用次数，小数部分表示冷却比例，范围[0,~]
    /// </summary>
    public class SkillColumnStorable : SkillColumnBase
    {
        [SerializeField] private Text StoredTime;
        /// <summary>
        /// 整数部分表示可用次数，小数部分表示冷却比例，范围[0,~]
        /// </summary>
        public override void SetAvailableTime(float time)
        {
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
public class SkillStorableController : SkillBaseController
{
    private SkillColumnStorable skill;
    private int maxStoreTime;
    private float cd;
    private float storeTime;
    public override void Update()
    {
        storeTime += Time.deltaTime / cd;
        if (storeTime > maxStoreTime) storeTime = maxStoreTime;
        if (skill != null)
        {
            skill.SetAvailableTime(storeTime);
        }
    }
    public override bool CanUse()
    {
        return storeTime >= 0.999f && base.CanUse();
    }
    public override void OnUse()
    {
        storeTime -= 1f;
        base.OnUse();
    }
    public static SkillBaseController Create(int index, Target t, int maxStoreTime, float cd)
    {
        var r = new SkillStorableController();
        r.target = t;
        r.SkillIndex = index;
        if (t && t is PlayerData p)
        {
            r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.Storable) as SkillColumnStorable;
            r.skill.SetSprite(Tool.SpriteManager.GetSprite(VarietyManager.GetSkill(index).sprite));
        }
        r.maxStoreTime = maxStoreTime;
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}