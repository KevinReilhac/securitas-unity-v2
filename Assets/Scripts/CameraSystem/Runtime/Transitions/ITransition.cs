using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spop.CameraSystem
{
    public interface ITransition
    {
        public UniTask PlayTransition(ACameraSpot startCameraSpot, ACameraSpot endCameraSpot, Camera camera, GameObject transitionGameObject);
        public void Cancel();
        public bool IsRunning { get; }
    }
}
