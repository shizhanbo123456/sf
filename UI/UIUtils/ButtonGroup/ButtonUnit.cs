using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonStateable))]
public class ButtonUnit:MonoBehaviour
{
    private ButtonGroup buttonGroup;
    [HideInInspector]public Button button;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            if (UseColor)
            {
                if (selected) button.image.color = SelectedCol;
                else button.image.color = NormalCol;
            }
            if (UseSprite)
            {
                if (selected) button.image.sprite = SelectedSp;
                else button.image.sprite = NormalSp;
            }
        }
    }
    private bool selected;
    [Space]
    public bool UseColor = false;//---------------------
    public Color NormalCol;
    public Color SelectedCol;
    [Space]
    public bool UseSprite = false;//--------------------
    public Sprite NormalSp;
    public Sprite SelectedSp;

    [HideInInspector]public int index;
    
    public void Init(ButtonGroup buttongroup)
    {
        buttonGroup = buttongroup;
        button = GetComponent<ButtonStateable>();
        button.onClick.AddListener(() => { if (buttonGroup) buttonGroup.Select(index); });
        Selected = false;
    }
}
