using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPVE : Level
{
    [HideInInspector] public Monster Monster;

    public override bool ProcessCampData(Dictionary<int, int> data)
    {
        bool signal = false;
        Dictionary<int, int> d2 = new Dictionary<int, int>();
        foreach (var i in data.Keys)
        {
            if (data[i] != 1)
            {
                signal = true;
            }
            d2.Add(i, 1);
        }
        foreach (var i in d2.Keys)
        {
            data[i] = d2[i];
        }
        if (signal) Tool.Notice.ShowMesg("我方已自动调整为黄队");
        return true;
    }
    public override Func<bool> GetEndCondition()
    {
        return () =>
        {
            return Monster != null && !Monster.gameObject.activeSelf;
        };
        throw new Exception("错误的子模式");
    }
    public override void CalculateScore(out int killscore, out int alivescore, out int score)
    {
        PlayerData d = Tool.SceneController.Player.GetComponent<PlayerData>();
        int die = KilledCount[d.Camp];

        int mDeath = 5;
        killscore = Tool.AttributesManager.GetLevel() * 100;
        score = Tool.AttributesManager.GetLevel() * 100;
        if (die >= mDeath) alivescore = 0;
        else alivescore = (int)((1 - (die * 1f / mDeath)) * score);
        if (TimeUsed > 600) score = 0;
        else score = (int)((1 - TimeUsed / 600f) * score);
    }
}
