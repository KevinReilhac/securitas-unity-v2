using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Spop.AreaSystem.UI
{
    public class NavigationScreen : MonoBehaviour
    {
        public enum ScreenType
        {
            Area,
            InterestPoint,
        }

        [Header("References")]
        [SerializeField] private Button backButton;
        [SerializeField] private AreaPanel areaPanel;
        [SerializeField] private InterestPointPanel interestPointPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;

        private void Awake()
        {
            AreaManager.instance.OnAreaChanged += OnAreaChanged;
            areaPanel.onLineClick += OnAreaLineClick;
            backButton.onClick.AddListener(OnPressedBackButton);
        }

        private void OnAreaLineClick(baseArea area)
        {
            AreaManager.instance.SetCurrentArea(area);
        }

        private void OnPressedBackButton()
        {
            if (AreaManager.instance.CurrentArea.ParentArea == null)
            {
                SceneManager.LoadScene(MenuSettings.Instance.mainMenuScene.ScenePath);
                return;
            }

            AreaManager.instance.SetCurrentArea(AreaManager.instance.CurrentArea.ParentArea);
        }

        private void OnAreaChanged(baseArea from, baseArea to)
        {
            titleText.text = to.DisplayTitle;
            subtitleText.text = to.Subtitle;

            if (to is InterestPoint interestPoint)
            {
                SetScreen(ScreenType.InterestPoint);
                interestPointPanel.SetInterestPoint(interestPoint);
            }
            else if (to is Area area)
            {
                SetScreen(ScreenType.Area);
                areaPanel.SetArea(area);
            }
        }

        public void SetScreen(ScreenType screenType)
        {
            areaPanel.gameObject.SetActive(screenType == ScreenType.Area);
            interestPointPanel.gameObject.SetActive(screenType == ScreenType.InterestPoint);
        }
    }
}
