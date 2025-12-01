using UnityEngine;

namespace Spop.CameraSystem
{
    [System.Serializable]
    public class CameraSpotSettings
    {
        public float fov = 90f;
        public float nearClipPlane = 0.1f;
        public float farClipPlane = 100f;

        public CameraSpotSettings()
        {
            fov = 90f;
            nearClipPlane = 0.1f;
            farClipPlane = 1000f;
        }

        public CameraSpotSettings(float fov, float nearClipPlane, float farClipPlane)
        {
            this.fov = fov;
            this.nearClipPlane = nearClipPlane;
            this.farClipPlane = farClipPlane;
        }

        public CameraSpotSettings(Camera camera)
        {
            fov = camera.fieldOfView;
            nearClipPlane = camera.nearClipPlane;
            farClipPlane = camera.farClipPlane;
        }
    }
}
