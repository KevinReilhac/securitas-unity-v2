using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;
using System.Linq;
using System.Collections.Generic;

namespace Spop.CameraSystem
{
    public class SplineTransition : ITransition
    {
        public Sequence tween;
        public UniTask uniTask;

        private ACameraSpot startCameraSpot;
        private ACameraSpot endCameraSpot;
        private GameObject transitionGameObject;
        private SplineContainer splineContainer;

        public UniTask PlayTransition(ACameraSpot startCameraSpot, ACameraSpot endCameraSpot, Camera camera, GameObject transitionGameObject)
        {
            CameraSystemSettings settings = CameraSystemSettings.Instance;
            CameraSpotSettings startSettings = new CameraSpotSettings(camera);

            this.startCameraSpot = startCameraSpot;
            this.endCameraSpot = endCameraSpot;
            this.transitionGameObject = transitionGameObject;
            this.splineContainer = transitionGameObject.GetComponent<SplineContainer>();

            if (splineContainer == null)
            {
                Debug.LogError("SplineContainer not found on transition game object");
                camera.transform.position = endCameraSpot.GetStartPosition();
                camera.transform.rotation = endCameraSpot.GetStartRotation();
                return UniTask.CompletedTask;
            }

            Vector3 cameraStartPosition = camera.transform.position;
            Quaternion cameraStartRotation = camera.transform.rotation;

            Debug.Log("Spline animation started", transitionGameObject);
            tween = DOTween.Sequence();

            Vector3 startPosition = splineContainer.EvaluatePosition(0);
            Vector3? startSplineTangent = GetTengentDirection(splineContainer.Spline, 0, out Vector3 startTangentValue);
            Quaternion startSplineRotation = startSplineTangent.HasValue ? Quaternion.LookRotation(startTangentValue, Vector3.up) : endCameraSpot.GetStartRotation();

            Vector3? endSplineTangent = GetTengentDirection(splineContainer.Spline, 1, out Vector3 endTangentValue);
            Quaternion endSplineRotation = endSplineTangent.HasValue ? Quaternion.LookRotation(endTangentValue, Vector3.up) : endCameraSpot.GetStartRotation();

            //Move to start position
            tween.Append(DOVirtual.Float(0, 1,settings.TransitionDurationValue, (progress) =>
            {
                camera.transform.position = Vector3.Lerp(cameraStartPosition, startPosition, progress);
                camera.transform.rotation = Quaternion.Lerp(cameraStartRotation, startSplineRotation, progress);
            })).SetEase(settings.transitionEase);

            //Move along the spline
            tween.Append(DOVirtual.Float(0, 1, settings.TransitionDurationValue, (progress) =>
            {
                camera.transform.position = splineContainer.EvaluatePosition(progress);
                camera.transform.rotation = Quaternion.Lerp(startSplineRotation, endSplineRotation, progress);

                endCameraSpot.EvaluateCustomSettings(progress, startSettings, camera);
            })).SetEase(settings.transitionEase);

            //Move to end position
            tween.Append(DOVirtual.Float(0, 1, settings.TransitionDurationValue, (progress) =>
            {
                camera.transform.position = Vector3.Lerp(splineContainer.EvaluatePosition(1), endCameraSpot.GetStartPosition(), progress);
                camera.transform.rotation = Quaternion.Lerp(endSplineRotation, endCameraSpot.GetStartRotation(), progress);
            }).SetEase(settings.transitionEase));


            return UniTask.CompletedTask;
        }

        private Vector3? GetTengentDirection(Spline spline, float progress, out Vector3 tangent)
        {
            tangent = spline.EvaluateTangent(progress);
            if (tangent.magnitude > 0.001f)
            {
                tangent.Normalize();
                return tangent;
            }
            return null;
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