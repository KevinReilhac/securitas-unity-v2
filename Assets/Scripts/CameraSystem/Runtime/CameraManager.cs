using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spop.CameraSystem
{
    public class CameraManager : Manager<CameraManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private ACameraSpot defaultCameraSpot;
        public List<CameraSpotPeerTransition> cameraSpotPeerTransitions;

        private TransitionManager _transitionManager = new TransitionManager();
        private ITransition lastTransition;

        private ACameraSpot _activeCameraSpot;
        public ACameraSpot activeCameraSpot
        {
            get => _activeCameraSpot;
            set
            {
                SetActiveCameraSpot(value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _transitionManager.Setup(cameraSpotPeerTransitions);
        }

        private void Start()
        {
            SetActiveCameraSpot(defaultCameraSpot, false);
        }
        public async void SetActiveCameraSpot(ACameraSpot newCameraSpot, bool animate = true)
        {
            await SetActiveCameraSpotTask(newCameraSpot, animate);
        }

        public async UniTask SetActiveCameraSpotTask(ACameraSpot newCameraSpot, bool animate = true)
        {
            if (newCameraSpot == _activeCameraSpot)
                return;
            ACameraSpot oldCameraSpot = _activeCameraSpot;
            _activeCameraSpot = newCameraSpot;
            _activeCameraSpot.OnSetActive();

            if (lastTransition != null)
                lastTransition.Cancel();

            ITransition transition;
            GameObject transitionGameObject;
            if (animate && _transitionManager.GetTransition(oldCameraSpot, newCameraSpot, out transition, out transitionGameObject))
            {
                lastTransition = transition;
                await lastTransition.PlayTransition(oldCameraSpot, newCameraSpot, mainCamera, transitionGameObject);
            }
            else
            {
                mainCamera.transform.position = newCameraSpot.GetStartPosition();
                mainCamera.transform.rotation = newCameraSpot.GetStartRotation();
            }
        }

        private void Update()
        {
            if (lastTransition == null || !lastTransition.IsRunning)
            {
                mainCamera.transform.position = _activeCameraSpot.GetPosition();
                mainCamera.transform.rotation = _activeCameraSpot.GetRotation();
            }
        }

        public void MoveCamera(Vector2 delta)
        {
            if (_activeCameraSpot != null)
            {
                delta *= CameraSystemSettings.Instance.MoveSpeedValue;
                _activeCameraSpot.Move(delta * Time.deltaTime);
                _activeCameraSpot.StopAnimation();
            }
        }

        public void ZoomCamera(float delta)
        {
            if (_activeCameraSpot != null)
            {
                delta *= CameraSystemSettings.Instance.PinchSpeedMultiplier;
                _activeCameraSpot.Zoom(delta * Time.deltaTime);
            }
        }
    }
}
