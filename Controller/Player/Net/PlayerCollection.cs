using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        PlayerData playerdata=GetComponent<PlayerData>();
        playerdata.Init(data);
    }
}
