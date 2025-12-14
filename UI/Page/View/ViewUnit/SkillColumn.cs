using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SF.UI.Skill
{
    public class SkillColumn : MonoBehaviour
    {
        [SerializeField] private Image MainIcon;
        [SerializeField] private Text Key;
        [SerializeField] protected Image PieShade;

        [SerializeField] protected Text StoredTime;
        [SerializeField] protected GameObject StoredTimeObject;
        private bool labelActive;
        public virtual void SetKey(KeyCode key)//直接在View设置
        {
            Key.text = key.ToString();
        }
        public void SetSprite(Sprite sprite)
        {
            MainIcon.sprite = sprite;
        }
        public void SetAvailableTime(float time)
        {
            if (time < 0.0001f) PieShade.fillAmount = 1f;
            else PieShade.fillAmount = (1-time) % 1;
            if (labelActive) StoredTime.text = ((int)time).ToString();
        }
        public void SetLabelActive(bool active)
        {
            labelActive = active;
            StoredTimeObject.SetActive(active);
        }
    }
}