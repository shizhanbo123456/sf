using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGuard : Level
{
    public override bool ProcessCampData(Dictionary<int, int> data)
    {
        int bluecount = 0;
        int redcount = 0;
        foreach (var i in data)
        {
            if (i.Value != 0 && i.Value != 3)
            {
                Tool.Notice.ShowMesg("存在不正确的阵营(仅限红蓝)");
                return false;
            }
            else if (i.Value == 0)
            {
                redcount += 1;
            }
            else if (i.Value == 3)
            {
                bluecount += 1;
            }
        }
        if (bluecount > 1)
        {
            Tool.Notice.ShowMesg("蓝队人数过多");
            return false;
        }
        else if (bluecount < 1)
        {
            Tool.Notice.ShowMesg("蓝队人数过少");
            return false;
        }
        if (redcount < 1)
        {
            Tool.Notice.ShowMesg("红队人数过少");
            return false;
        }
        return true;
    }
    public override Func<bool> GetEndCondition()
    {
        if (submode == 0) return () =>
        {
            if (KilledCount[3] >= 1) return true;
            int b = 0;
            foreach (var i in Ore.Ores.Values)
            {
                if (i.Shengming <= 1) b += 1;
            }
            return b == Ore.Ores.Count;
        };
        if (submode == 1) return () =>
        {
            if (Tool.FightController.FightTimeCount > 360) return true;
            int b = 0;
            foreach (var i in Ore.Ores.Values)
            {
                if (i.Shengming <= 1) b += 1;
            }
            return b == Ore.Ores.Count;
        };
        if (submode == 2) return () =>
        {
            int b = 0;
            foreach (var i in Ore.Ores.Values)
            {
                if (i.Shengming <= 1) b += 1;
            }
            return b == Ore.Ores.Count;
        };
        if (submode == 3) return () =>
        {
            if (KilledCount[3] >= 1) return true;
            if (Tool.FightController.FightTimeCount > 360) return true;
            int b = 0;
            foreach (var i in Ore.Ores.Values)
            {
                if (i.Shengming <= 1) b += 1;
            }
            return b == Ore.Ores.Count;
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


        var h = Ore.OreHealthRate(out int max);//平均剩余比例
        float s = 0;
        foreach (var i in h) s += i.DedicatedAttributes.Shengming.Value;
        s /= (max * h.Count);
        if (d.Camp == 3)
        {
            score = (int)(s * 1500 + TimeUsed * 5 + kill * 225);
        }
        else
        {
            score = (int)(1 - s) * 2000;
            if (TimeUsed < 300)
            {
                score += 1000;
            }
            else if (TimeUsed < 600)
            {
                score += (int)(1000f * (600 - TimeUsed) / 3f);
            }
        }
    }
}
