using Spop.CameraSystem;
using UnityEditor;
using UnityEngine;

namespace Spop.AreaSystem.Editors
{
    public class AreaMenuItems
    {
        private const string AREA_ICON = "sv_label_1"; // Blue Label
        private const string INTEREST_POINT_ICON = "sv_label_6"; // Red Label

        [MenuItem("Tools/Area System/Rebuild Areas Tree")]
        public static void RebuildAreaTree()
        {
            Area[] areas = GameObject.FindObjectsByType<Area>(FindObjectsSortMode.None);

            foreach (Area area in areas)
            {
                area.RebuildAreaTree();
                Debug.Log("Rebuild Area Tree for " + area.name);
                EditorUtility.SetDirty(area);
            }
        }

        private static GameObject CreateAreaWithCamera<T>(MenuCommand menuCommand) where T : baseArea
        {
            GameObject areaObject = new GameObject("New Area");

            T areaComponent = areaObject.AddComponent<T>();

            GameObject cameraObject = new GameObject("Area Camera");
            cameraObject.transform.SetParent(areaObject.transform);
            OrbitalCameraSpot areaCamera = cameraObject.AddComponent<OrbitalCameraSpot>();

            areaComponent.SetAreaName(areaObject.name);
            areaComponent.SetCameraSpot(areaCamera);

            GameObjectUtility.SetParentAndAlign(areaObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(areaObject, "Create " + areaObject.name);

            Selection.activeObject = areaObject;
            areaObject.transform.localPosition = Vector3.zero;

            return areaObject;
        }

        [MenuItem("GameObject/Area System/Area", priority = 10)]
        private static void CreateAreaWithCamera(MenuCommand menuCommand)
        {
            GameObject areaObject = CreateAreaWithCamera<Area>(menuCommand);
            EditorGUIUtility.SetIconForObject(areaObject, EditorGUIUtility.IconContent(AREA_ICON).image as Texture2D);
            areaObject.name = "New Area";
        }

        [MenuItem("GameObject/Area System/Interest Point", priority = 11)]
        private static void CreateInterestPoint(MenuCommand menuCommand)
        {
            GameObject areaObject = CreateAreaWithCamera<InterestPoint>(menuCommand);
            EditorGUIUtility.SetIconForObject(areaObject, EditorGUIUtility.IconContent(INTEREST_POINT_ICON).image as Texture2D);
            areaObject.name = "New Interest Point";
        }
    }
}