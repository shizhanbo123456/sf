using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStateable : Button
{
    /// <summary>
    /// 0 Normal  1 Highlighted  2 Pressed  3 Disabled
    /// </summary>
    public int GetState()
    {
        return (int)currentSelectionState;
    }
    public bool Pressed()
    {
        return GetState() == 2;
    }
}
