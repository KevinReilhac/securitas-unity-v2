using System.Collections.Generic;
using UnityEngine;

namespace Spop.AreaSystem
{
    /// <summary>
    /// Represents an interest point in the scene.
    /// Interest points are areas that contains more information about the area.
    /// </summary>
    public class InterestPoint : baseArea
    {
        [SerializeField] private InterestPointInfos interestPointInfos;
        [SerializeField] private float spacingOffset = 0;

        public InterestPointInfos InterestPointInfos => interestPointInfos;
        public float SpacingOffset => spacingOffset;
    }
}
