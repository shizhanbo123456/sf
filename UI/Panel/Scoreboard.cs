using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]private List<Text> Column0 = new List<Text>();
    [SerializeField]private List<Text> Column1 = new List<Text>();
    [SerializeField]private List<Text> Column2 = new List<Text>();

    private List<List<Text>> Data;

    public void ShowPanel(string[] verticalHeaders, string[] horizontalHeaders)
    {
        Column0[0].text = string.Empty;
        for(int i = 1; i < 4; i++)
        {
            Column0[i].text=verticalHeaders[i - 1];
        }
        Column1[0].text = horizontalHeaders[0];
        Column2[0].text = horizontalHeaders[1];

        for (int i = 1; i < 4; i++)
        {
            Column1[i].text = "0";
            Column2[i].text = "0";
        }
        gameObject.SetActive(true);
    }
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
    public void SetText(int x, int y, string str)
    {
        if (Data == null) 
            Data = new List<List<Text>>()
            {
                Column0,Column1,Column2
            };
        Data[x][y].text = str;
    }
}
