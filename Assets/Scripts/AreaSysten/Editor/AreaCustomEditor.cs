using UnityEngine;
using UnityEditor;
using System.Runtime.Remoting.Messaging;
using Spop.CameraSystem.Editors.UI;

namespace Spop.AreaSystem.Editors
{
    public class baseAreaCustomEditor<T> : Editor where T : baseArea
    {
        protected T area;

        // SerializedProperty for each field in baseArea
        protected SerializedProperty areaNameProperty;
        protected SerializedProperty displayTitleProperty;
        protected SerializedProperty subtitleProperty;
        protected SerializedProperty cameraSpotProperty;
        protected SerializedProperty customDotPositionProperty;
        protected SerializedProperty eventsProperty;

        protected virtual void OnEnable()
        {
            area = (T)target;

            // Initialize SerializedProperties
            areaNameProperty = serializedObject.FindProperty("areaName");
            displayTitleProperty = serializedObject.FindProperty("displayTitle");
            subtitleProperty = serializedObject.FindProperty("subtitle");
            cameraSpotProperty = serializedObject.FindProperty("cameraSpot");
            customDotPositionProperty = serializedObject.FindProperty("customDotPosition");
            eventsProperty = serializedObject.FindProperty("events");

            
        }

        public override void OnInspectorGUI()
        {
            DrawInfoProperties();

            DrawReferencesProperties();

            DrawSpace();
            EditorGUILayout.PropertyField(eventsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInfoProperties()
        {
            DrawHeader("Info");

            DrawAreaName();
            EditorGUILayout.PropertyField(displayTitleProperty);
            EditorGUILayout.PropertyField(subtitleProperty);
        }

        protected void DrawAreaName()
        {
            EditorGUILayout.BeginHorizontal();
            string newAreaName = EditorGUILayout.TextField("Area Name", area.AreaName);
            if (newAreaName != area.AreaName)
            {
                RenameObjects(newAreaName, area.GetType().Name);
                areaNameProperty.stringValue = newAreaName;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RenameObjects(string newName, string type)
        {
            area.name = string.Format("{0} {1}", newName, type);
            area.CameraSpot.name = string.Format("{0} Camera Spot", newName);
        }

        protected virtual void DrawReferencesProperties()
        {
            DrawHeader("References");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(cameraSpotProperty);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_editicon.sml"), GUILayout.Width(20f)))
            {
                CameraSystemConfiguratorWindow.ShowWindow(area.CameraSpot);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(customDotPositionProperty);
            if (customDotPositionProperty.objectReferenceValue == null)
            {
                if (GUILayout.Button("+", GUILayout.Width(20f)))
                    CreateCustomDotPosition();
            }
            else
            {
                if (GUILayout.Button("Focus", GUILayout.Width(50f)))
                {
                    Selection.activeTransform = (Transform)customDotPositionProperty.objectReferenceValue;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreateCustomDotPosition()
        {
            if (customDotPositionProperty.objectReferenceValue == null)
            {
                GameObject customDotPosition = new GameObject($"{area.AreaName} Dot Position");
                customDotPosition.transform.SetParent(area.transform);
                customDotPosition.transform.localPosition = Vector3.zero;
                customDotPosition.transform.localRotation = Quaternion.identity;
                customDotPosition.transform.localScale = Vector3.one;
                customDotPositionProperty.objectReferenceValue = customDotPosition.transform;
            }
        }

        protected void DrawHeader(string title)
        {
            DrawSpace();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        protected void DrawSpace()
        {
            EditorGUILayout.Space(10);
        }
    }

    [CustomEditor(typeof(Area))]
    public class AreaCustomEditor : baseAreaCustomEditor<Area>
    {
        private SerializedProperty descriptionProperty;
        private SerializedProperty subdescriptionProperty;
        private SerializedProperty subAreasProperty;

        protected override void OnEnable()
        {
            descriptionProperty = serializedObject.FindProperty("description");
            subdescriptionProperty = serializedObject.FindProperty("subdescription");
            subAreasProperty = serializedObject.FindProperty("subAreas");
            base.OnEnable();
        }

        protected override void DrawReferencesProperties()
        {
            base.DrawReferencesProperties();

            DrawSubAreas();
        }

        private void DrawSubAreas()
        {
            DrawSpace();
            EditorGUILayout.PropertyField(subAreasProperty);
            if (GUILayout.Button("Refresh Sub Areas"))
            {
                area.RebuildAreaTree();
                EditorUtility.SetDirty(area);
            }
        }

        protected override void DrawInfoProperties()
        {
            base.DrawInfoProperties();
            EditorGUILayout.PropertyField(descriptionProperty);
            EditorGUILayout.PropertyField(subdescriptionProperty);
        }
    }

    [CustomEditor(typeof(InterestPoint))]
    public class InterestPointCustomEditor : baseAreaCustomEditor<InterestPoint>
    {
        private SerializedProperty interestPointInfosProperty;
        private SerializedProperty spacingOffsetProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            interestPointInfosProperty = serializedObject.FindProperty("interestPointInfos");
            spacingOffsetProperty = serializedObject.FindProperty("spacingOffset");
        }

        protected override void DrawInfoProperties()
        {
            base.DrawInfoProperties();
            EditorGUILayout.PropertyField(interestPointInfosProperty);
            EditorGUILayout.PropertyField(spacingOffsetProperty);
        }
    }
}
