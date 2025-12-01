using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spop.AreaSystem.Editors
{
    public static class AreaEditorExtention
    {
        public static void RebuildAreaTree(this Area area)
        {
            List<baseArea> subAreas = new List<baseArea>();

            foreach (Transform child in area.transform)
            {
                baseArea subArea = child.GetComponent<baseArea>();
                if (subArea != null)
                    subAreas.Add(subArea);
            }
            area.SetSubAreas(subAreas);
        }
    }
}