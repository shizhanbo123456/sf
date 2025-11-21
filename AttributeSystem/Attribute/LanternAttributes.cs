using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem.Attributes
{
    [CreateAssetMenu(menuName = "Attributes/LanternAttributes")]
    public class LanternAttributes : TargetAttributes
    {
        [Space]
        public LinerGrowth RegenerationTime;
    }
}