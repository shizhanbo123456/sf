using LevelCreator.TargetTemplate;
using UnityEngine;

public struct TimeLineData
{
    public TimeLineData(Target t)
    {
        Target = t;
        index = 0;
        pos= new Vector3(0, 0, 0);
    }
    public TimeLineData(Target t,int i)
    {
        Target = t;
        index = i;
        pos = new Vector3(0, 0, 0);
    }
    public TimeLineData(Target t,Vector3 p)
    {
        Target = t;
        index = 0;
        pos = p;
    }
    public TimeLineData(Target t,int i,Vector3 p)
    {
        Target = t;
        index = i;
        pos = p;
    }
    public Target Target;
    public int index;
    public Vector3 pos;
}