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
        [SerializeField] private Text KeyOutline;
        protected float AvailableTime;
        public static Dictionary<KeyCode, string> KeytoStr = new Dictionary<KeyCode, string>()
    {
        { KeyCode.J, "J" },
        { KeyCode.U, "U" },
        { KeyCode.I, "I" },
        { KeyCode.O, "O" },
        { KeyCode.L, "L" },
        { KeyCode.H, "H" },
    };
        public virtual void Init(KeyCode key, Sprite mainIcon)
        {
            Key.text = KeytoStr[key];
            KeyOutline.text = KeytoStr[key];
            MainIcon.sprite = mainIcon;
        }
        public abstract void SetAvailableTime(float time);
    }
}