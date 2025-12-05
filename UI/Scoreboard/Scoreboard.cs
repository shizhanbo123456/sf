using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]private List<Text> Column0 = new List<Text>();
    [SerializeField]private List<Text> Column1 = new List<Text>();

    private List<List<Text>> Data;

    private void OnEnable()
    {
        foreach (var i in Column0) i.text = "0";
        foreach (var i in Column1) i.text = "0";
    }
    public void SetText(int x, int y, string str)
    {
        if (Data == null) 
            Data = new List<List<Text>>()
            {
                Column0,Column1
            };
        Data[x][y].text = str;
    }
}
