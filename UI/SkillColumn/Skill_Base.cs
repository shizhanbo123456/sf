using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Skill
{
    public abstract class Skill_Base : MonoBehaviour
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
    public abstract class SkillBaseContrller
    {
        protected int SkillIndex;
        public abstract bool CanUse();
        public virtual void OnUse()
        {

        }
    }
}