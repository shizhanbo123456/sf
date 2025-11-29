using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    [CreateAssetMenu(menuName = "Attributes/MonsterAttributes")]
    public class MonsterAttributes : TargetAttributes
    {
        [Space]
        public LinerGrowth StateInterval;
    }
}