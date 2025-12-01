using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Spop.CameraSystem.Editors
{
    public static class SplineToolSelection
    {
        public static void ActivateSplineEditingTool()
        {
            // Set the current tool to Custom to enable custom tools
            Tools.current = Tool.Custom;
            
            // Look specifically for edit/select tools, not create tools
            try
            {
                var splinesAssembly = System.Reflection.Assembly.Load("Unity.Splines.Editor");
                if (splinesAssembly != null)
                {
                    var splineToolTypes = splinesAssembly.GetTypes();
                    
                    // First pass: Look for tools specifically named for editing/selecting
                    foreach (var type in splineToolTypes)
                    {
                        if (type.IsSubclassOf(typeof(EditorTool)))
                        {
                            // Prioritize tools that are clearly for editing, not creating
                            if ((type.Name.Contains("Select") || type.Name.Contains("Edit") || 
                                 type.Name.Contains("Manipulator") || type.Name.Contains("Transform") ||
                                 (type.Name.Contains("Knot") && !type.Name.Contains("Placement") && !type.Name.Contains("Insert"))) &&
                                !type.Name.Contains("Create") && !type.Name.Contains("Draw") && !type.Name.Contains("Placement"))
                            {
                                ToolManager.SetActiveTool(type);
                                Debug.Log($"Activated spline editing tool: {type.Name}");
                                return;
                            }
                        }
                    }
                    
                    // Second pass: Look for any spline tool that's likely for editing
                    foreach (var type in splineToolTypes)
                    {
                        if (type.IsSubclassOf(typeof(EditorTool)) && 
                            type.Name.Contains("Spline") &&
                            !type.Name.Contains("Create") && !type.Name.Contains("Draw") && 
                            !type.Name.Contains("Placement") && !type.Name.Contains("Insert"))
                        {
                            ToolManager.SetActiveTool(type);
                            Debug.Log($"Activated spline tool: {type.Name}");
                            return;
                        }
                    }
                    
                    // Third pass: Just list all available spline tools for debugging
                    Debug.Log("Available spline tools:");
                    foreach (var type in splineToolTypes)
                    {
                        if (type.IsSubclassOf(typeof(EditorTool)) && 
                            (type.Name.Contains("Spline") || type.Name.Contains("Knot")))
                        {
                            Debug.Log($"- {type.Name}");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not load Unity.Splines.Editor assembly: {e.Message}");
            }

            Debug.LogWarning("Could not find or activate spline editing tool. Please manually select the spline edit tool from the toolbar.");
        }
    }
}