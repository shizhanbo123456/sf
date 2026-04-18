using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public class AutoController : TargetController
    {
        private float aggressiveness=0.6f;
        private enum MonsterState
        {
            Relax, RandomMove, MoveTowardPlayer
        }
        private float stateTimeLeft;

        public override void Init(Target t, Dictionary<TargetParams, string> param)
        {
            base.Init(t, param);
            if(param.ContainsKey(TargetParams.Aggressiveness)) float.TryParse(param[TargetParams.Aggressiveness], out aggressiveness);
            SwitchState(MonsterState.Relax);
        }
        protected override void Update()
        {
            base.Update();
            StateUpdate();
        }
        private void StateUpdate()
        {
            stateTimeLeft -= Time.deltaTime;
            if (stateTimeLeft < 0) SwitchState(GetRandomState());
        }
        private MonsterState GetRandomState()
        {
            var v = Random.value;
            if (v < aggressiveness) return MonsterState.MoveTowardPlayer;
            if (v >= 0.8f) return MonsterState.RandomMove;
            return MonsterState.Relax;
        }
        private Vector2Int GetVToNearestPlayer()
        {
            const int xthreshold = 5;
            const int ythreshold = 2;
            var t = target.GetNearestEnemy();
            if (!t) return new Vector2Int();
            int x = 0;
            int y = 0;
            if (t.transform.position.x < transform.position.x - xthreshold) x = -1;
            else if (t.transform.position.x > transform.position.x + xthreshold) x = 1;
            if (t.transform.position.y < transform.position.y - ythreshold) y = -1;
            else if (t.transform.position.y > transform.position.y + ythreshold) y = 1;
            return new Vector2Int(x, y);
        }
        private void SwitchState(MonsterState state)
        {
            if (target.OperationLock.LockedInHierechy)
            {
                stateTimeLeft = 0.2f;
                return;
            }
            switch (state)
            {
                case MonsterState.Relax:
                    inputVector = new Vector2Int();
                    stateTimeLeft = 2;
                    break;
                case MonsterState.RandomMove:
                    inputVector = new Vector2Int(Random.Range(-1, 2), 0);
                    stateTimeLeft = 2;
                    break;
                case MonsterState.MoveTowardPlayer:
                    inputVector = GetVToNearestPlayer();
                    stateTimeLeft = 0.5f;
                    break;
            }
        }

        private Vector2Int inputVector;
        public override Vector2Int GetInputVector()
        {
            return inputVector;
        }
    }
}