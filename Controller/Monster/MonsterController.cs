using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Variety.Template;

public class MonsterController : TargetController
{
    private bool CanMove;
    private bool CanFly;
    private enum MonsterState
    {
        Relax,RandomMove,MoveTowardPlayer
    }
    private MonsterState presentState;
    private float stateTimeLeft;

    private float interval;
    private float useSkillCD;

    public override void Init(Target t)
    {
        base.Init(t);
        interval = (t as Monster).StateInterval;
        CanMove = (t as Monster).MonsterCanMove;
        CanFly = (t as Monster).MonsterCanFly;
        if (!CanMove)
        {
            ApplyMotion(new MotionStatic(99999999, true, 99999999));
        }
        else SwitchState(MonsterState.Relax);
    }
    private void SwitchState(MonsterState state)
    {
        if (target.OperationLock.LockedInHierechy)
        {
            stateTimeLeft = 0.2f;
            return;
        }
        presentState = state;

        if (CanFly)
        {

            switch (state)
            {
                case MonsterState.Relax: 
                    ApplyMotion(new MotionStatic(99999, true, 1)); 
                    stateTimeLeft = 5; 
                    break;
                case MonsterState.RandomMove:
                    float x = Random.Range(-1, 2);
                    float y = Random.Range(-1, 2);
                    ApplyMotion(new MotionDir(new Vector2(x,y), 99999, true, 1)); 
                    stateTimeLeft = 2; 
                    break;
                case MonsterState.MoveTowardPlayer: 
                    ApplyMotion(new MotionDir((Vector2)GetVToNearestPlayer() * MoveSpeed, 99999, true, 1)); 
                    stateTimeLeft = 0.5f; 
                    break;
            }
        }
        else
        {
            

            switch (state)
            {
                case MonsterState.Relax:
                    inputVector = new Vector2Int(); 
                    stateTimeLeft = 5; 
                    break;
                case MonsterState.RandomMove:
                    inputVector = new Vector2Int(Random.Range(-1, 2), 0); 
                    stateTimeLeft = 2; 
                    break;
                case MonsterState.MoveTowardPlayer:
                    inputVector= GetVToNearestPlayer(); 
                    stateTimeLeft = 0.5f; 
                    break;
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        StateUpdate();
    }
    private void StateUpdate()
    {
        if (CanMove)
        {
            stateTimeLeft -= Time.deltaTime;
            if (stateTimeLeft < 0) SwitchState(GetRandomState());
        }

        useSkillCD -= Time.deltaTime;
        if (useSkillCD < 0) UseSkill();
    }
    private MonsterState GetRandomState()
    {
        var v = Random.Range(0, 27);
        if (v >= 25) return MonsterState.Relax;
        if (v >= 20) return MonsterState.RandomMove;
        return MonsterState.MoveTowardPlayer;
    }
    private Vector2Int GetVToNearestPlayer()
    {
        const int xthreshold= 5;
        const int ythreshold= 2;
        var t=target.GetNearestEnemy();
        int x = 0;
        int y = 0;
        if (t.transform.position.x < transform.position.x - xthreshold) x = -1;
        else if (t.transform.position.x > transform.position.x + xthreshold) x = 1;
        if (t.transform.position.y < transform.position.y - ythreshold) y = -1;
        else if (t.transform.position.y > transform.position.y + ythreshold) y = 1;
        return new Vector2Int(x, y);
    }

    private Vector2Int inputVector;
    public override Vector2Int GetInputVector()
    {
        return inputVector;
    }

    private int skillIndex;
    private void UseSkill()
    {
        if (useSkillCD > 0) return;
        var b=target.skillController.UseSkill(skillIndex);
        if (!b)
        {
            useSkillCD = 0.3f;
        }
        else
        {
            useSkillCD = interval;
        }
        skillIndex++;
        if(skillIndex==target.skillController.Skills.Count)skillIndex = 0;
    }
}
