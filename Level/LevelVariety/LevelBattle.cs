using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBattle : Level
{
    public override bool ProcessCampData(Dictionary<int, int> data)
    {
        return true;
    }
    public override Func<bool> GetEndCondition()
    {
        if (submode == 0) return () =>
        {
            foreach (var i in KillCount) if (i >= 30) return true;
            return false;
        };
        if (submode == 1) return () =>
        {
            foreach (var i in KillCount) if (i >= 60) return true;
            return false;
        };
        if (submode == 2) return () =>
        {
            foreach (var i in KillCount) if (i >= 100) return true;
            return false;
        };
        if (submode == 3) return () =>
        {
            if (Tool.FightController.FightTimeCount >= 300) return true;
            return false;
        };
        if (submode == 4) return () =>
        {
            if (Tool.FightController.FightTimeCount >= 450) return true;
            return false;
        };
        if (submode == 5) return () =>
        {
            if (Tool.FightController.FightTimeCount >= 600) return true;
            return false;
        };
        throw new Exception("错误的子模式");
    }
    public override void CalculateScore(out int killscore, out int alivescore, out int score)
    {
        PlayerData d = Tool.SceneController.Player.GetComponent<PlayerData>();
        int kill = KillCount[d.Camp];
        int die = KilledCount[d.Camp];
        score = 0;

        killscore = kill * 50;
        alivescore = 2000 / Mathf.Max(die, 1);

        switch (submode)
        {
            case 0: score = kill * 60; break;
            case 1: score = kill * 30; break;
            case 2: score = kill * 15; break;
            case 3: score = kill * 60; break;
            case 4: score = kill * 30; break;
            case 5: score = kill * 15; break;
        }
    }
}
