using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;

public class SubInput : MonoBehaviour
{
    [SerializeField] private ButtonStateable Left;
    [SerializeField] private ButtonStateable Right;
    [SerializeField] private ButtonStateable Jump;
    [SerializeField] private ButtonStateable Fall;

    private List<bool>SkillUseSignal=new List<bool>() { false,false,false,false,false,false};

    //0 Windows   1 Android
    private TargetPlatform InputMode
    {
        get { return Instance.Platform; }
    }


    private void Awake()
    {
        Tool.SubInput = this;
    }

    private float time_tem=-10f;
    private int dir_tem;
    public int HorizontalInput()
    {
        if(InputMode==0)return (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        int i = (Left.GetState() == 2) ? -1 : 0 + ((Right.GetState() == 2) ? 1 : 0);
        if (i !=0 || Time.time-time_tem>0.1f)
        {
            dir_tem = i;
        }
        if(i!=0)time_tem = Time.time;
        return dir_tem;
    }

    public bool JumpSignal()
    {
        if (InputMode == 0) return Input.GetKeyDown(KeyCode.K);
        return false;
    }
    public bool FallSignal()
    {
        if (InputMode == 0) return Input.GetKey(KeyCode.S);
        return Fall.GetState()==2;
    }

    public bool CanUseSkill(int index)
    {
        if (InputMode == 0)
        {
            return Input.GetKeyDown(LevelCreator.TargetTemplate.PlayerSkillController.Keys[index]);
        }
        if (SkillUseSignal[index])
        {
            SkillUseSignal[index] = false;
            return true;
        }
        return false;
    }
    public void AndroidUseSkill(int index)
    {
        SkillUseSignal[index] = true;
    }
}