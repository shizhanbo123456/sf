using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TransitionView : BasePage
{
    [SerializeField]private Text loadingText;
    [SerializeField]private Text LabelText;
    private void Awake()
    {
        loadingText.text = "Мгдижа...";
    }
    public override void Repaint()
    {
        LabelText.text = TransitionController.Instance.Label;
    }
}