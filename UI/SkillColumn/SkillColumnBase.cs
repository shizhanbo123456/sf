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
    }
}
public abstract class SkillBaseController
{
    public int SkillIndex { get; protected set; }
    public virtual void Update()
    {

    }
    public abstract bool CanUse();
    public virtual void OnUse()
    {

    }
}