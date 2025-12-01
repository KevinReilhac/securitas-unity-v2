using Spop.CameraSystem.Editors.UI;
using UnityEditor;
using UnityEngine;

namespace Spop.CameraSystem.Editors
{
    [CustomEditor(typeof(ACameraSpot), true)]
    public class CameraSpotCustomEditor : Editor
    {
        private ACameraSpot cameraSpot;

        private void OnEnable()
        {
            cameraSpot = (ACameraSpot)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Edit in CameraSystemConfigurator"))
            {
                CameraSystemConfiguratorWindow.ShowWindow(cameraSpot);
            }
        }
    }
}
