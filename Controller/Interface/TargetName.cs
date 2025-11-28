using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetName : MonoBehaviour
{
    [SerializeField] private Text Recog;
    [SerializeField] private Image Back;
    public string text
    {
        set
        {
            Recog.text = value;
            GUIStyle style = new GUIStyle()
            {
                font = Recog.font,
                fontSize = Recog.fontSize,
                fontStyle = Recog.fontStyle
            };
            Texture2D tex = new Texture2D(1024, 16);
            Vector2 size = style.CalcSize(new GUIContent(Recog.text));
            Destroy(tex);
            Back.transform.localScale = new Vector3((size.x + 30f) / 12000f, 0.008f, 1);
        }
    }
    public Color color
    {
        set
        {
            Recog.color = value;
        }
    }
}
