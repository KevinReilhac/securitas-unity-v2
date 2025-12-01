using System;
using System.Collections.Generic;
using DevLocker.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuSettings;

namespace Spop.AreaSystem.UI
{

    public class MainMenuPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI subdescriptionText;
        [SerializeField] private Transform linesContainer;

        [Header("Prefabs")]
        [SerializeField] private AreaPanelLine areaPanelLinePrefab;

        private List<AreaPanelLine> lines = new List<AreaPanelLine>();
        public Action<SceneReference> onLineClick;

        private void Awake()
        {
            DisableTemplateLines();
            ClearLines();
            SetupLines();
        }

        private void SetupLines()
        {
            foreach (SceneItem sceneItem in MenuSettings.Instance.sceneItems)
            {
                SceneItem tmpSceneItem = sceneItem;
                AreaPanelLine lineInstance = Instantiate(areaPanelLinePrefab, linesContainer);
                lineInstance.Setup(tmpSceneItem.title, () =>
                {
                    SceneManager.LoadScene(tmpSceneItem.sceneReference.ScenePath);
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
