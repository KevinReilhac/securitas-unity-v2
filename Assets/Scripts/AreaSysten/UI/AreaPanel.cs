using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Spop.AreaSystem.UI
{

    public class AreaPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI subdescriptionText;
        [SerializeField] private Transform linesContainer;

        [Header("Prefabs")]
        [SerializeField] private AreaPanelLine areaPanelLinePrefab;

        private List<AreaPanelLine> lines = new List<AreaPanelLine>();
        public Action<baseArea> onLineClick;

        private void Awake()
        {
            DisableTemplateLines();
        }

        public void SetArea(Area area)
        {
            descriptionText.text = area.Description;
            subdescriptionText.text = area.Subdescription;

            ClearLines();

            foreach (baseArea subArea in area.SubAreas)
            {
                baseArea tmpSubArea = subArea;
                AreaPanelLine lineInstance = Instantiate(areaPanelLinePrefab, linesContainer);
                lineInstance.Setup(subArea.AreaName, () =>
                {
                    onLineClick?.Invoke(tmpSubArea);
                });
                lines.Add(lineInstance);
            }
        }

        private void ClearLines()
        {
            foreach (AreaPanelLine line in lines)
            {
                Destroy(line.gameObject);
            }

            lines.Clear();
        }

        private void DisableTemplateLines()
        {
            foreach (Transform child in linesContainer)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
