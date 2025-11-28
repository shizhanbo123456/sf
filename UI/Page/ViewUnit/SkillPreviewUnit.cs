using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Variety.Base;

public class SkillPreviewUnit : MonoBehaviour
{
    public Action<SkillBase> OnClicked;
    public SkillBase skill;
    public Image skillIcon;
    public void InvokeEvent()
    {
        OnClicked?.Invoke(skill);
    }
}
