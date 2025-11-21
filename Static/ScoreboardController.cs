using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    public GameObject Prefab;

    public Transform ScoreboardRoot
    {
        get
        {
            return Tool.PageManager.PlayModePage.ScoreboardRoot;
        }
    }
    public Scoreboard Scoreboard;


    private void Awake()
    {
        Tool.ScoreboardController = this;
    }

    public void InitScoreboard()
    {
        Scoreboard = Instantiate(Prefab, ScoreboardRoot).GetComponent<Scoreboard>();
        Scoreboard.transform.localPosition = new Vector3();
    }
    public void SetText(int x, int y, string data)
    {
        if (Scoreboard == null) return;
        Scoreboard.SetText(x, y, data);
    }
    public void CloseScoreBoard()
    {
        if(Scoreboard) Destroy(Scoreboard.gameObject);
    }
}
