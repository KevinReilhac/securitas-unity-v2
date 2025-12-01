using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Spop.CameraSystem
{
    public abstract class ACameraSpot : MonoBehaviour
    {
        public delegate void PositionChangedDelegate();
        public PositionChangedDelegate OnPositionChanged;

        [SerializeField] protected CameraSpotSettings cameraSettings;
        [Header("Animation")]
        private bool animated = true;

        public abstract Vector3 GetStartPosition();
        public abstract Quaternion GetStartRotation();
        public abstract void ResetPosition();
        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();

        public virtual void Move(Vector2 dragDelta) { /* Empty implementation */ }
        public virtual void Zoom(float pinchDelta) { /* Empty implementation */ }
        protected Coroutine animationCoroutine;

        public virtual void OnSetActive()
        {
            Debug.Log("OnSetActive");
            ResetPosition();
            animationCoroutine = StartCoroutine(StartAnimation());
        }

        public virtual void OnSetInactive()
        {
            Debug.Log("OnSetInactive");
            StopAnimation();
        }

        public bool IsActive() => CameraManager.instance.activeCameraSpot == this;
        public void SetActive() => CameraManager.instance.SetActiveCameraSpot(this);

        public virtual void EvaluateCustomSettings(float progress, CameraSpotSettings startSettings, Camera camera)
        {
            camera.fieldOfView = Mathf.Lerp(startSettings.fov, cameraSettings.fov, progress);
            camera.nearClipPlane = Mathf.Lerp(startSettings.nearClipPlane, cameraSettings.nearClipPlane, progress);
            camera.farClipPlane = Mathf.Lerp(startSettings.farClipPlane, cameraSettings.farClipPlane, progress);
        }

        public CameraSpotSettings GetCameraSpotSettings()
        {
            return cameraSettings;
        }

        private IEnumerator StartAnimation()
        {
            Debug.Log("Delay Start");
            yield return Delay();
            Debug.Log("Delay End");
            Debug.Log("Animation Start");
            yield return Animation();
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(CameraSystemSettings.Instance.AnimationDelayValue);
        }

        public void StopAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }

        protected virtual IEnumerator Animation()
        {
            yield break;
        }

#if UNITY_EDITOR
        public abstract void EditorMove(Vector2 dragDelta);
#endif
    }
}
