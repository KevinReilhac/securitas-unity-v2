using System;
using System.Collections.Generic;
using DevLocker.Utils;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuSettings;

namespace Spop.AreaSystem.UI
{

    public class MainMenuPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
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
            SetupTitles();
        }

        private void SetupTitles()
        {
            SetupText(MenuSettings.Instance.title, titleText);
            SetupText(MenuSettings.Instance.subtitle, subtitleText);
            SetupText(MenuSettings.Instance.descriptionText, descriptionText);
            SetupText(MenuSettings.Instance.subdescriptionText, subdescriptionText);
        }

        private void SetupText(string text, TextMeshProUGUI textMeshProUGUI)
        {
            if (string.IsNullOrEmpty(text))
            {
                textMeshProUGUI.gameObject.SetActive(false);
            }
            else
            {
                textMeshProUGUI.gameObject.SetActive(true);
                textMeshProUGUI.text = text;
            }
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
