using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Spop.CameraSystem.Inputs
{
    public class TouchScreenCameraInputsDebugScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private Slider pinchSpeedSlider;
        [SerializeField] private TextMeshProUGUI pinchSpeedText;
        [SerializeField] private Slider dragSpeedSlider;
        [SerializeField] private TextMeshProUGUI dragSpeedText;
        [SerializeField] private TextMeshProUGUI transitionTimeText;
        [SerializeField] private Slider transitionTimeSlider;
        [SerializeField] private TextMeshProUGUI animationDelayText;
        [SerializeField] private Slider animationDelaySlider;
        [SerializeField] private TextMeshProUGUI animationLeftRightDelayText;
        [SerializeField] private Slider animationLeftRightDelaySlider;
        [SerializeField] private TextMeshProUGUI animationSpeedText;
        [SerializeField] private Slider animationSpeedSlider;
        [SerializeField] private TextMeshProUGUI activeCamera;
        [SerializeField] private TextMeshProUGUI targetCameraPosition;

        private StringBuilder debugTextBuilder = new StringBuilder();
        private float lastPinchValue;
        private Vector2 lastDragValue;
        private Vector2 lastPrimaryTouchPosition;
        private Vector2 lastSecondaryTouchPosition;

        // FPS tracking
        private float deltaTime = 0.0f;
        private float fps = 0.0f;

        private TouchScreenCameraInputs touchScreenCameraInputs;

        private void OnEnable()
        {
            if (touchScreenCameraInputs == null)
                touchScreenCameraInputs = FindFirstObjectByType<TouchScreenCameraInputs>();

            touchScreenCameraInputs.OnPinch += OnPinch;
            touchScreenCameraInputs.OnDrag += OnDrag;
            touchScreenCameraInputs.OnPrimaryTouch += OnPrimaryTouch;
            touchScreenCameraInputs.OnSecondaryTouch += OnSecondaryTouch;
            pinchSpeedSlider.onValueChanged.AddListener(OnPinchSpeedChanged);
            dragSpeedSlider.onValueChanged.AddListener(OnDragSpeedChanged);
            transitionTimeSlider.onValueChanged.AddListener(OnTransitionTimeChanged);
            animationDelaySlider.onValueChanged.AddListener(OnAnimationDelayChanged);
            animationLeftRightDelaySlider.onValueChanged.AddListener(OnAnimationLeftRightDelayChanged);
            animationSpeedSlider.onValueChanged.AddListener(OnAnimationSpeedChanged);
            CameraManager.instance.OnCameraSpotChanged += OnCameraChanged;

            pinchSpeedSlider.value = CameraSystemSettings.Instance.PinchSpeedMultiplier;
            dragSpeedSlider.value = CameraSystemSettings.Instance.MoveSpeedMultiplier;
            transitionTimeSlider.value = CameraSystemSettings.Instance.TransitionDurationMultiplier;
            animationDelaySlider.value = CameraSystemSettings.Instance.AnimationDelayMultiplier;
            animationLeftRightDelaySlider.value = CameraSystemSettings.Instance.AnimationLeftRightDelayMultiplier;
            animationSpeedSlider.value = CameraSystemSettings.Instance.AnimationSpeedMultiplier;
            OnCameraChanged(CameraManager.instance.activeCameraSpot);
        }

        private void OnCameraChanged(ACameraSpot spot)
        {
            if (spot == null)
                SetActiveCamera("NULL");
            else
                SetActiveCamera(spot.name);
        }

        private void SetActiveCamera(string cameraName)
        {
            activeCamera.text = string.Format("Active Camera: {0}", cameraName);
        }

        private void SetTargetCameraPosition(ACameraSpot cameraSpot)
        {
            if (cameraSpot == null)
                targetCameraPosition.text = "Non orbital Camera";
            else if (cameraSpot is OrbitalCameraSpot orbitalCameraSpot)
                targetCameraPosition.text = string.Format("Target position: {0}", new Vector2(orbitalCameraSpot.targetXPosition, orbitalCameraSpot.targetYPosition));
            else
                targetCameraPosition.text = "Non orbital Camera";
        }

        private void OnDisable()
        {
            touchScreenCameraInputs.OnPinch -= OnPinch;
            touchScreenCameraInputs.OnDrag -= OnDrag;
            touchScreenCameraInputs.OnPrimaryTouch -= OnPrimaryTouch;
            touchScreenCameraInputs.OnSecondaryTouch -= OnSecondaryTouch;
            pinchSpeedSlider.onValueChanged.RemoveListener(OnPinchSpeedChanged);
            dragSpeedSlider.onValueChanged.RemoveListener(OnDragSpeedChanged);
            transitionTimeSlider.onValueChanged.RemoveListener(OnTransitionTimeChanged);
            animationDelaySlider.onValueChanged.RemoveListener(OnAnimationDelayChanged);
            animationLeftRightDelaySlider.onValueChanged.RemoveListener(OnAnimationLeftRightDelayChanged);
            animationSpeedSlider.onValueChanged.RemoveListener(OnAnimationSpeedChanged);
        }

        private void OnAnimationDelayChanged(float value)
        {
            CameraSystemSettings.Instance.AnimationDelayMultiplier = value;
            animationDelayText.text = string.Format("x{0}", value.ToString("0.000"));
        }
        
        private void OnAnimationLeftRightDelayChanged(float value)
        {
            CameraSystemSettings.Instance.AnimationLeftRightDelayMultiplier = value;
            animationLeftRightDelayText.text = string.Format("x{0}", value.ToString("0.000"));
        }
        
        private void OnAnimationSpeedChanged(float value)
        {
            CameraSystemSettings.Instance.AnimationSpeedMultiplier = value;
            animationSpeedText.text = string.Format("x{0}", value.ToString("0.000"));
        }

        private void OnPrimaryTouch(Vector2 vector)
        {
            lastPrimaryTouchPosition = vector;
        }

        private void OnSecondaryTouch(Vector2 vector)
        {
            lastSecondaryTouchPosition = vector;
        }

        private void OnDragSpeedChanged(float value)
        {
            CameraSystemSettings.Instance.MoveSpeedMultiplier = value;
            dragSpeedText.text =string.Format("x{0}", value.ToString("0.000"));
        }

        private void OnPinchSpeedChanged(float value)
        {
            CameraSystemSettings.Instance.PinchSpeedMultiplier = value;
            pinchSpeedText.text = string.Format("x{0}", value.ToString("0.000"));
        }

        private void OnTransitionTimeChanged(float arg0)
        {
            CameraSystemSettings.Instance.TransitionDurationMultiplier = arg0;
            transitionTimeText.text = string.Format("x{0}", arg0.ToString("0.000"));
        }

        private void OnDrag(Vector2 vector)
        {
            lastDragValue = vector;
        }

        private void OnPinch(float obj)
        {
            lastPinchValue = obj;
        }

        private void Update()
        {
            // Calculate FPS
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;

            debugTextBuilder.Clear();
            debugTextBuilder.AppendLine($"Images par seconde: {fps:0.0}");
            debugTextBuilder.AppendLine();
            debugTextBuilder.AppendLine($"Mode: {touchScreenCameraInputs.CurrentMode}");
            debugTextBuilder.AppendLine($"Nombre de touches: {Touch.activeTouches.Count}");
            debugTextBuilder.AppendLine($"Position du toucher principal: {lastPrimaryTouchPosition}");
            debugTextBuilder.AppendLine($"Position du toucher secondaire: {lastSecondaryTouchPosition}");
            debugTextBuilder.AppendLine($"Valeur du drag: {lastDragValue}");
            debugTextBuilder.AppendLine($"Valeur du pinch: {lastPinchValue}");
            debugText.text = debugTextBuilder.ToString();
            SetTargetCameraPosition(CameraManager.instance.activeCameraSpot);
        }
    }
}
