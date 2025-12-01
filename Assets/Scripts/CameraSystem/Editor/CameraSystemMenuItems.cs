using UnityEngine;
using UnityEditor;

namespace Spop.CameraSystem.Editors
{
    public static class CameraSystemMenuItems
    {
        [MenuItem("Tools/CameraSystem/Select Active Camera Spot")]
        public static void SelectActiveCameraSpot()
        {
            if (SelectActiveCameraSpotValidate())
            {
                Selection.activeGameObject = CameraManager.instance.activeCameraSpot.gameObject;
            }
            else
            {
                Debug.LogWarning("No active camera spot found");
            }
        }

        [MenuItem("Tools/Camera System/Select Active Camera Spot", true)]
        public static bool SelectActiveCameraSpotValidate()
        {
            return Application.isPlaying && CameraManager.HasInstance && CameraManager.instance.activeCameraSpot != null;
        }
    }
}
