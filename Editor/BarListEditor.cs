using SF.UI.Bar;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BarList))]
public class BarListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BarList barList = (BarList)target;
        if (GUILayout.Button("Layout Bars"))
        {
            barList.LayoutBars();
        }
    }
}
