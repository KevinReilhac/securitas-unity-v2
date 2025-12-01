using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Spop.CameraSystem
{
    public class TweenTransition : ITransition
    {
        public Tween tween;
        public UniTask uniTask;

        public UniTask PlayTransition(ACameraSpot startCameraSpot, ACameraSpot endCameraSpot, Camera camera, GameObject transitionGameObject)
        {
            CameraSystemSettings settings = CameraSystemSettings.Instance;
            CameraSpotSettings startSettings = new CameraSpotSettings(camera);
            Vector3 cameraStartPosition = camera.transform.position;
            Quaternion cameraStartRotation = camera.transform.rotation;


            tween = DOVirtual.Float(0f, 1f, settings.TransitionDurationValue, (progress) =>
            {
                camera.transform.position = Vector3.Lerp(cameraStartPosition, endCameraSpot.GetStartPosition(), progress);
                camera.transform.rotation = Quaternion.Lerp(cameraStartRotation, endCameraSpot.GetStartRotation(), progress);
                endCameraSpot.EvaluateCustomSettings(progress, startSettings, camera);
            }).SetEase(settings.transitionEase);

            uniTask = tween.AsyncWaitForCompletion().AsUniTask();
            return uniTask;
        }

        public void Cancel()
        {
            if (IsRunning)
            {
                tween.Kill();
            }
        }

        public bool IsRunning => tween != null && tween.IsActive();
    }
}