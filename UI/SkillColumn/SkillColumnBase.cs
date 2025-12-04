using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Skill
{
    public abstract class SkillColumnBase : MonoBehaviour
    {
        [SerializeField] private Image MainIcon;
        [SerializeField] protected Image PieShade;
        [SerializeField] private Text Key;
        public virtual void SetKey(KeyCode key)
        {
            Key.text = key.ToString();
        }
        public abstract void SetAvailableTime(float time);
        public void SetSprite(Sprite sprite)
        {
            MainIcon.sprite = sprite;
        }
    }
}
public abstract class SkillBaseController
{
    public int SkillIndex { get; protected set; }
    protected Target target;
    public virtual void Update()
    {

    }
    public virtual bool CanUse()
    {
        var skill = VarietyManager.GetSkill(SkillIndex);
        if (!skill.CanUse(target)) return false;
        return true;
    }
    public virtual void OnUse()
    {

    }
    public virtual void OnDiscard()
    {

    }
}