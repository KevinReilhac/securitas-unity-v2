using UnityEngine;

namespace Spop.CameraSystem
{
    [System.Serializable]
    public class CameraSpotPeerTransition
    {
        public ACameraSpot startCameraSpot;
        public ACameraSpot endCameraSpot;
        public ETransitionTypes transitionType = ETransitionTypes.Tween;
        public GameObject transitionGameObject;
        public bool bothWays = true;
    }
}