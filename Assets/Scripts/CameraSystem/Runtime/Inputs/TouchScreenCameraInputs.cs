using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


namespace Spop.CameraSystem
{
    public class TouchScreenCameraInputs : MonoBehaviour
    {
        public enum ECameraInputMode
        {
            Drag,
            Pinch,
        }

        [SerializeField] private CameraManager cameraManager;


        public ECameraInputMode CurrentMode { get; private set; } = ECameraInputMode.Drag;
        public Vector2? lastPrimaryTouchPosition { get; private set; }
        public float? lastPinchDistance { get; private set; }
        public event Action<float> OnPinch;
        public event Action<Vector2> OnDrag;
        public event Action<Vector2> OnSecondaryTouch;
        public event Action<Vector2> OnPrimaryTouch;

        public CameraSystemSettings settings => CameraSystemSettings.Instance;

        public void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }

        public void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            TouchSimulation.Disable();
        }

        public void Update()
        {
            switch (CurrentMode)
            {
                case ECameraInputMode.Drag:
                    UpdateDragMode();
                    break;
                case ECameraInputMode.Pinch:
                    UpdatePinchMode();
                    break;
            }

            if (Keyboard.current != null)
                KeyboardControlls();
        }

        private void KeyboardControlls()
        {
            float keyboardZoom = 0;
            Vector2 keyboardMove = Vector2.zero;

            if (Keyboard.current.pageDownKey.isPressed)
                keyboardZoom -= 1;
            if (Keyboard.current.pageUpKey.isPressed)
                keyboardZoom += 1;

            if (Keyboard.current.leftArrowKey.isPressed)
                keyboardMove.x -= 1;
            if (Keyboard.current.rightArrowKey.isPressed)
                keyboardMove.x += 1;

            if (Keyboard.current.downArrowKey.isPressed)
                keyboardMove.y -= 1;
            if (Keyboard.current.upArrowKey.isPressed)
                keyboardMove.y += 1;

            if (keyboardZoom != 0)
                SendPinchInput(keyboardZoom * settings.keyboardZoomSpeedMultiplier);
            if (keyboardMove != Vector2.zero)
                SendDragInput(keyboardMove * settings.keyboardDragSpeedMultiplier);
        }

        private void UpdatePinchMode()
        {
            if (Touch.activeTouches.Count == 0)
            {
                ChangeMode(ECameraInputMode.Drag);
                return;
            }

            if (Touch.activeTouches.Count == 2)
            {
                Touch primaryTouch = Touch.activeTouches[0];
                Touch secondaryTouch = Touch.activeTouches[1];

                // If the touch is not moved, do not send any input
                if (!primaryTouch.phase.Equals(TouchPhase.Moved) || !secondaryTouch.phase.Equals(TouchPhase.Moved))
                    return;

                OnSecondaryTouch?.Invoke(secondaryTouch.screenPosition);
                OnPrimaryTouch?.Invoke(primaryTouch.screenPosition);

                float currentPinchDistance = Vector2.Distance(primaryTouch.screenPosition, secondaryTouch.screenPosition);
                if (lastPinchDistance.HasValue)
                {
                    float delta = currentPinchDistance - lastPinchDistance.Value;
                    SendPinchInput(delta * settings.PinchSpeedValue);
                }
                lastPinchDistance = currentPinchDistance;
            }
        }

        private void UpdateDragMode()
        {
            if (Touch.activeTouches.Count == 2)
            {
                ChangeMode(ECameraInputMode.Pinch);
                return;
            }

            if (Touch.activeTouches.Count == 1)
            {
                Touch primaryTouch = Touch.activeTouches[0];

                if (IsOverUI(primaryTouch))
                    return;

                OnPrimaryTouch?.Invoke(primaryTouch.screenPosition);

                if (lastPrimaryTouchPosition.HasValue)
                {
                    Vector2 delta = primaryTouch.screenPosition - lastPrimaryTouchPosition.Value;
                    SendDragInput(delta * settings.MoveSpeedValue);
                }
                lastPrimaryTouchPosition = primaryTouch.screenPosition;
            }

            if (Touch.activeTouches.Count == 0)
            {
                lastPrimaryTouchPosition = null;
            }
        }

        private bool IsOverUI(Touch touch)
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId);
        }

        private void ChangeMode(ECameraInputMode newMode)
        {
            if (newMode == ECameraInputMode.Pinch)
            {
                CurrentMode = ECameraInputMode.Pinch;
                lastPinchDistance = null;
            }
            else
            {
                CurrentMode = ECameraInputMode.Drag;
                lastPrimaryTouchPosition = null;
            }

            Debug.Log($"Changed mode to {newMode}");
        }

        private void SendDragInput(Vector2 delta)
        {
            OnDrag?.Invoke(delta);
            cameraManager.MoveCamera(delta);
        }

        private void SendPinchInput(float delta)
        {
            OnPinch?.Invoke(delta);
            cameraManager.ZoomCamera(delta);
        }


    }
}
