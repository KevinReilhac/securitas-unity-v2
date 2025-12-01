using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spop.CameraSystem
{
    public class JumpTransition : ITransition
    {
        public UniTask PlayTransition(ACameraSpot startCameraSpot, ACameraSpot endCameraSpot, Camera camera, GameObject transitionGameObject)
        {
            CameraSpotSettings startSettings = new CameraSpotSettings(camera);
            camera.transform.position = endCameraSpot.GetStartPosition();
            camera.transform.rotation = endCameraSpot.GetStartRotation();
            endCameraSpot.EvaluateCustomSettings(1f, startSettings, camera);
            return UniTask.CompletedTask;
        }

        public void Cancel()
        {
            // Do nothing
        }

        public bool IsRunning => false;
    }
}
