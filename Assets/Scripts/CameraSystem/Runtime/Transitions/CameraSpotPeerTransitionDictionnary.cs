using System.Collections.Generic;
using UnityEngine;

namespace Spop.CameraSystem
{
    [System.Serializable]
    public struct CameraSpotPair
    {
        public ACameraSpot from;
        public ACameraSpot to;

        public CameraSpotPair(ACameraSpot from, ACameraSpot to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Equals(object obj)
        {
            if (obj is CameraSpotPair other)
            {
                return from == other.from && to == other.to;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(from, to);
        }
    }

    public class CameraSpotPeerTransitionDictionnary
    {
        private Dictionary<CameraSpotPair, ETransitionTypes> _transitions;
        private Dictionary<CameraSpotPair, GameObject> _transitionGameObjects;

        public CameraSpotPeerTransitionDictionnary()
        {
            _transitions = new Dictionary<CameraSpotPair, ETransitionTypes>();
            _transitionGameObjects = new Dictionary<CameraSpotPair, GameObject>();
        }

        public void Setup(List<CameraSpotPeerTransition> peerTransitions)
        {
            _transitions.Clear();
            _transitionGameObjects.Clear();

            foreach (var transition in peerTransitions)
            {
                if (transition.startCameraSpot == null || transition.endCameraSpot == null)
                    continue;

                // Add the primary direction
                var primaryPair = new CameraSpotPair(transition.startCameraSpot, transition.endCameraSpot);
                _transitions[primaryPair] = transition.transitionType;
                _transitionGameObjects[primaryPair] = transition.transitionGameObject;

                // Add reverse direction if bothWays is true
                if (transition.bothWays)
                {
                    var reversePair = new CameraSpotPair(transition.endCameraSpot, transition.startCameraSpot);
                    _transitions[reversePair] = transition.transitionType;
                    _transitionGameObjects[reversePair] = transition.transitionGameObject;
                }
            }
        }

        public bool TryGetTransition(ACameraSpot from, ACameraSpot to, out ETransitionTypes transitionType, out GameObject transitionGameObject)
        {
            var pair = new CameraSpotPair(from, to);
            transitionType = _transitions.GetValueOrDefault(pair, ETransitionTypes.Tween);
            transitionGameObject = _transitionGameObjects.GetValueOrDefault(pair, null);
            return true;
        }

        public bool GetTransition(ACameraSpot from, ACameraSpot to, out ETransitionTypes transitionType, out GameObject transitionGameObject, ETransitionTypes defaultType = ETransitionTypes.Tween)
        {
            if (TryGetTransition(from, to, out transitionType, out transitionGameObject))
            {
                return true;
            }
            else
            {
                transitionType = defaultType;
                transitionGameObject = null;
            }

            return false;
        }

        public bool HasTransition(ACameraSpot from, ACameraSpot to)
        {
            var pair = new CameraSpotPair(from, to);
            return _transitions.ContainsKey(pair);
        }

        public void AddTransition(ACameraSpot from, ACameraSpot to, ETransitionTypes transitionType, bool bothWays = false)
        {
            var pair = new CameraSpotPair(from, to);
            _transitions[pair] = transitionType;

            if (bothWays)
            {
                var reversePair = new CameraSpotPair(to, from);
                _transitions[reversePair] = transitionType;
            }
        }

        public void RemoveTransition(ACameraSpot from, ACameraSpot to, bool bothWays = false)
        {
            var pair = new CameraSpotPair(from, to);
            _transitions.Remove(pair);

            if (bothWays)
            {
                var reversePair = new CameraSpotPair(to, from);
                _transitions.Remove(reversePair);
            }
        }

        public void Clear()
        {
            _transitions.Clear();
        }
    }
}
