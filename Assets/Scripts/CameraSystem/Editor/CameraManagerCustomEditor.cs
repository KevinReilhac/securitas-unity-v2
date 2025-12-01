using Spop.CameraSystem.Editors.UI;
using UnityEditor;
using UnityEngine;

namespace Spop.CameraSystem.Editors
{
    [CustomEditor(typeof(CameraManager), true)]
    public class CameraManagerCustomEditor : Editor
    {
        private CameraManager cameraManager;
        private SerializedProperty defaultCameraSpot;

        private void OnEnable()
        {
            cameraManager = (CameraManager)target;
            defaultCameraSpot = serializedObject.FindProperty("defaultCameraSpot");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(defaultCameraSpot);

            if (GUILayout.Button("Edit Transitions"))
            {
                CameraSystemConfiguratorWindow.ShowWindow();
            }
        }
    }
}
