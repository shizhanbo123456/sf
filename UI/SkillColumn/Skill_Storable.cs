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
    public class Skill_Storable : SkillColumnBase
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
    private Skill_Storable skill;
    private PlayerData Player;
    private int cost;
    private int maxStoreTime;
    private float cd;
    private float storeTime;
    public override void Update()
    {
        storeTime += Time.deltaTime / cd;
        if (storeTime > maxStoreTime) storeTime = maxStoreTime;
        if (Player && skill != null)
        {
            if (Player.Mofa >= cost) skill.SetAvailableTime(storeTime);
            else skill.SetAvailableTime(0);
        }
    }
    public override bool CanUse()
    {
        if (Player && Player.Mofa < cost) return false;
        return storeTime >= 0.999f;
    }
    public override void OnUse()
    {
        if (Player) Player.Mofa -= cost;
        storeTime -= 1f;
        base.OnUse();
    }
    public static SkillBaseController Create(int index, Target t, int cost, int maxStoreTime, float cd)
    {
        var r = new SkillStorableController();
        r.SkillIndex = index;
        if (t && t is PlayerData p)
        {
            r.skill = Tool.PageManager.PlayModePage.CreateSkillColumn(PlayModePage.SkillColumnType.Storable) as Skill_Storable;
            r.Player = p;
        }
        r.cost = cost;
        r.maxStoreTime = maxStoreTime;
        r.cd = cd;
        r.storeTime = 1;
        return r;
    }
}