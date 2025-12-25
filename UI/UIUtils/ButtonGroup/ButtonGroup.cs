using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    public List<ButtonUnit> buttonUnits = new List<ButtonUnit>();

    public bool StartChooseFirst = true;
    private bool Initialized = false;

    public int Choosed=0;

    public delegate void onselect(int x);
    public onselect OnSelect;


    private void OnEnable()
    {
        if (!Initialized)
            Init();
    }
    public void Init()
    {
        int i = 0;
        foreach (var buttonUnit in buttonUnits)
        {
            buttonUnit.Init(this);
            buttonUnit.index = i++;
        }
        if (StartChooseFirst) Select(0);
        Initialized = true;
    }
    public void Select(int x)//Ñ¡ÔñµÚx¸ö
    {
        if (!Initialized)
            Init();
        CancelSelect();
        Choosed = x;
        buttonUnits[x].Selected = true;
        OnSelect?.Invoke(x);
    }
    public void SelectAndInvoke(int x)
    {
        if (!Initialized)
            Init();
        Select(x);
        buttonUnits[x].button.onClick.Invoke();
        OnSelect?.Invoke(x);
    }
    public void CancelSelect()
    {
        if (!Initialized)
            Init();
        foreach (var b in buttonUnits) b.Selected = false;
    }
}
