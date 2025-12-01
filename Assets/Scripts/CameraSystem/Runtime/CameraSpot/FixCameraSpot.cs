using UnityEngine;

namespace Spop.CameraSystem
{
    public class FixCameraSpot : ACameraSpot
    {

        public override Vector3 GetPosition()
        {
            return GetStartPosition();
        }

        public override Quaternion GetRotation()
        {
            return GetStartRotation();
        }

        public override Vector3 GetStartPosition()
        {
            return transform.position;
        }

        public override Quaternion GetStartRotation()
        {
            return transform.rotation;
        }

        public override void ResetPosition()
        {
            // Do nothing
        }
#if UNITY_EDITOR
        public override void EditorMove(Vector2 dragDelta)
        {
            //Do nothing
        }
#endif
    }
}
