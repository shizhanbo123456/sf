using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonSkillPlayerCollection : EnsBehaviourCollection
{
    public NonSkillPlayerData PlayerData;
    protected override void Init(string data)
    {
        PlayerData.Init(data);
    }
}
