using System.Collections.Generic;
using UnityEngine;

namespace Spop.CameraSystem
{
    public class TransitionManager
    {
        private CameraSpotPeerTransitionDictionnary _cameraSpotPeerTransitionDictionnary = new CameraSpotPeerTransitionDictionnary();

        public void Setup(List<CameraSpotPeerTransition> cameraSpotPeerTransitions)
        {
            _cameraSpotPeerTransitionDictionnary.Setup(cameraSpotPeerTransitions);
        }

        public bool GetTransition(ACameraSpot startCameraSpot, ACameraSpot endCameraSpot, out ITransition transition, out GameObject transitionGameObject)
        {
            ETransitionTypes transitionType;
            if (_cameraSpotPeerTransitionDictionnary.GetTransition(startCameraSpot, endCameraSpot, out transitionType, out transitionGameObject, CameraSystemSettings.Instance.defaultTransitionType))
            {
                transition = GetTransitionFromTransitionType(transitionType);
                return true;
            }
            else
            {
                transition = null;
            }

            return false;
        }

        public ITransition GetTransitionFromTransitionType(ETransitionTypes transitionType)
        {
            switch (transitionType)
            {
                case ETransitionTypes.Tween:
                    return new TweenTransition();
                case ETransitionTypes.Spline:
                    return new SplineTransition();
                default:
                case ETransitionTypes.Jump:
                    return new JumpTransition();
            }
        }
    }
}
