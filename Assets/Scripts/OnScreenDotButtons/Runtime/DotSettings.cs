using UnityEngine;

namespace Scope.OnScreenDotButtons
{
    [CreateAssetMenu(fileName = "DotSettings", menuName = "ScriptableObjects/OnScreenDotButtons/DotSettings")]
    public class DotSettings : ScriptableObject
    {
        private static DotSettings instance;
        public static DotSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<DotSettings>("DotSettings");
                }
                return instance;
            }
        }

        [SerializeField] private Dot dotPrefab;
        [SerializeField] private float dotScale = 1f;

        public Dot DotPrefab => dotPrefab;
        public float DotScale => dotScale;
    }
}
