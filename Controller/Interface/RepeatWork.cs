using System.Collections.Generic;
using UnityEngine;
using Variety.Base;

public class RepeatWork:MonoBehaviour
{
    private List<RepeatContent> contents=new List<RepeatContent>();
    public void Add(RepeatContent content)
    {
        contents.Add(content);
    }
    private void Update()
    {
        foreach (var i in contents) i.Update();
    }
}