using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Spop.AreaSystem.Editors
{
    [CustomEditor(typeof(AreaManager))]
    public class AreaManagerCustomEditor : Editor
    {
        private AreaManager areaManager;
        private Dictionary<baseArea, bool> foldoutStates = new Dictionary<baseArea, bool>();

        private void OnEnable()
        {
            areaManager = (AreaManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            if (GUILayout.Button("Rebuild Area Tree"))
                AreaMenuItems.RebuildAreaTree();

            DrawAreaTree();
        }

        private void DrawAreaTree()
        {
            if (areaManager.CurrentArea != null)
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Current Area:", areaManager.CurrentArea.AreaName, EditorStyles.helpBox);
                GUILayout.Space(5);
            }

            GUILayout.Space(10);
            GUILayout.Label("Area Tree", EditorStyles.boldLabel);


            // Get the default area and check if it's an Area (has subareas)
            var defaultArea = areaManager.defaultArea;
            if (defaultArea != null)
            {
                if (defaultArea is Area area && area.SubAreas.Count > 0)
                {
                    DrawAreaNode(area, 0);
                }
                else
                {
                    EditorGUILayout.HelpBox("Default area has no subareas to display.", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No default area assigned.", MessageType.Warning);
            }
        }

        private void DrawAreaNode(Area area, int indentLevel)
        {
            EditorGUI.indentLevel = indentLevel;

            // Initialize foldout state if not exists
            if (!foldoutStates.ContainsKey(area))
                foldoutStates[area] = false;

            // Create a rect for the entire line to capture right-click events
            Rect lineRect = EditorGUILayout.GetControlRect();
            
            // Draw foldout if area has subareas
            bool hasSubAreas = area.SubAreas.Count > 0;
            if (hasSubAreas)
            {
                foldoutStates[area] = EditorGUI.Foldout(lineRect, foldoutStates[area], $"{area.AreaName} ({area.SubAreas.Count} subareas)");
            }
            else
            {
                EditorGUI.LabelField(lineRect, $"• {area.AreaName}");
            }

            // Handle right-click context menu
            HandleContextMenu(lineRect, area);

            // Draw children if foldout is open
            if (hasSubAreas && foldoutStates[area])
            {
                EditorGUI.indentLevel++;
                foreach (baseArea subArea in area.SubAreas)
                {
                    if (subArea == null) continue;

                    if (subArea is Area subAreaWithChildren)
                    {
                        DrawAreaNode(subAreaWithChildren, indentLevel + 1);
                    }
                    else
                    {
                        // For non-Area baseAreas (like InterestPoint)
                        DrawLeafNode(subArea, indentLevel + 1);
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel = 0;
        }

        private void DrawLeafNode(baseArea area, int indentLevel)
        {
            EditorGUI.indentLevel = indentLevel;

            // Create a rect for the entire line to capture right-click events
            Rect lineRect = EditorGUILayout.GetControlRect();
            
            string areaType = area.GetType().Name;
            EditorGUI.LabelField(lineRect, $"• {area.AreaName} ({areaType})");

            // Handle right-click context menu
            HandleContextMenu(lineRect, area);

            EditorGUI.indentLevel = 0;
        }

        private void HandleContextMenu(Rect rect, baseArea area)
        {
            Event currentEvent = Event.current;
            
            if (currentEvent.type == EventType.ContextClick && rect.Contains(currentEvent.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                
                menu.AddItem(new GUIContent("Edit"), false, () => EditArea(area));
                
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }

        private void EditArea(baseArea area)
        {
            Selection.activeGameObject = area.gameObject;
            EditorGUIUtility.PingObject(area.gameObject);
        }

        private void SelectArea(baseArea area)
        {
            Selection.activeGameObject = area.gameObject;
        }

        private void PingArea(baseArea area)
        {
            EditorGUIUtility.PingObject(area.gameObject);
        }
    }
}
