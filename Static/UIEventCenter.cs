using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EC
{
    public class UIEventCenter : MonoBehaviour
    {
        private void Awake()
        {
            Tool.UIEventCenter = this;
        }

        private Dictionary<string, Action<UIEvent>> Events = new Dictionary<string, Action<UIEvent>>();

        public void RegistEvent<T>(Action<T> action, bool allowRepeat = false) where T : UIEvent
        {
            var key = typeof(T).ToString();
            if (Events.ContainsKey(key))
            {
                if (!allowRepeat)
                {
                    Debug.LogWarning(key + "已经注册");
                    return;
                }
                Events[key] += e => { action.Invoke(e as T); };
            }
            else
            {
                Events.Add(key, e => { action.Invoke(e as T); });
            }
        }
        public void TrigEvent<T>(T eventbase) where T : UIEvent
        {
            var key = typeof(T).ToString();
            if (Events.ContainsKey(key))
                Events[key].Invoke(eventbase);
            else
                Debug.LogWarning("EventCenter缺少对" + key + "的响应");
        }
    }

    public abstract class UIEvent
    {

    }
}