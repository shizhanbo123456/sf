using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public List<Text> Column0 = new List<Text>();
    public List<Text> Column1 = new List<Text>();

    public List<List<Text>> Data;

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
