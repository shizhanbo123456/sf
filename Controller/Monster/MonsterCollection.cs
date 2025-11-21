using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollection : EnsBehaviourCollection
{
    protected override void Init(string data)
    {
        Monster m=GetComponent<Monster>();
        m.Init(data);
    }
}
