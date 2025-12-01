using DG.Tweening;
using UnityEngine;

namespace Spop.CameraSystem
{
    [CreateAssetMenu(fileName = "CameraSystemSettings", menuName = "Scriptable Objects/CameraSystemSettings")]
    public class CameraSystemSettings : ScriptableObject
    {
        private static CameraSystemSettings _instance = null;
        public static CameraSystemSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<CameraSystemSettings>("CameraSystemSettings");
                }
                return _instance;
            }
        }

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 1f;
        public Ease transitionEase = Ease.OutBack;
        public ETransitionTypes defaultTransitionType = ETransitionTypes.Tween;
        [Header("Movement")]
        [SerializeField] private float dragSpeedValue = 0.1f;
        [SerializeField] private float pinchSpeedValue = 0.1f;
        [Header("Animation")]
        [SerializeField] private float animationDelayValue = 0f;
        [SerializeField] private float animationSpeedValue = 1f;
        [SerializeField] private float animationLeftRightDelayValue = 2f;
        [Header("Keyboard")]
        public float keyboardZoomSpeedMultiplier = 0.1f;
        public float keyboardDragSpeedMultiplier = 0.1f;
        public float moveLerpSpeed = 10f;

        public float MoveSpeedMultiplier
        {
            set => PlayerPrefs.SetFloat("_MoveSpeedMultiplier", value);
            get => PlayerPrefs.GetFloat("_MoveSpeedMultiplier", 1f);
        }

        public float PinchSpeedMultiplier
        {
            set => PlayerPrefs.SetFloat("_PinchSpeedMultiplier", value);
            get => PlayerPrefs.GetFloat("_PinchSpeedMultiplier", 1f);
        }

        public float TransitionDurationMultiplier
        {
            set =>PlayerPrefs.SetFloat("_TransitionDurationMultiplier", value);
            get => PlayerPrefs.GetFloat("_TransitionDurationMultiplier", 1f);
        }

        public float AnimationDelayMultiplier
        {
            set => PlayerPrefs.SetFloat("_AnimationDelayMultiplier", value);
            get => PlayerPrefs.GetFloat("_AnimationDelayMultiplier", 1f);
        }

        public float AnimationSpeedMultiplier
        {
            set => PlayerPrefs.SetFloat("_AnimationSpeedMultiplier", value);
            get => PlayerPrefs.GetFloat("_AnimationSpeedMultiplier", 1f);
        }

        public float AnimationLeftRightDelayMultiplier
        {
            set => PlayerPrefs.SetFloat("_AnimationLeftRightDelayMultiplier", value);
            get => PlayerPrefs.GetFloat("_AnimationLeftRightDelayMultiplier", 1f);
        }

        public float MoveSpeedValue
        {
            get => dragSpeedValue * MoveSpeedMultiplier;
        }

        public float PinchSpeedValue
        {
            get => pinchSpeedValue * PinchSpeedMultiplier;
        }

        public float TransitionDurationValue
        {
            get => transitionDuration * TransitionDurationMultiplier;
        }
        
        public float AnimationDelayValue
        {
            get => animationDelayValue * AnimationDelayMultiplier;
        }

        public float AnimationSpeedValue
        {
            get => animationSpeedValue * AnimationSpeedMultiplier;
        }

        public float AnimationLeftRightDelayValue
        {
            get => animationLeftRightDelayValue * AnimationLeftRightDelayMultiplier;
        }
    }
}
