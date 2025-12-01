using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors
{
    public class CameraSettingsPanel
    {
        private const string ACTIVE_BUTTON_CLASS = "footer-enabled-button";
        private VisualElement root;
        private VisualElement cameraSettingsContainer;
        private Button showPanelButton;

        private Slider fovSlider;
        private FloatField fovField;
        private FloatField nearClipPlane;
        private FloatField farClipPlane;
        private VisualElement closePanelButton;

        private Action<CameraSpotSettings> onSettingsChangedCallback;

        private bool isActive => root.style.display == DisplayStyle.Flex;

        public CameraSettingsPanel(VisualElement root, Button showPanelButton, Action<CameraSpotSettings> onSettingsChangedCallback)
        {
            this.root = root;
            this.showPanelButton = showPanelButton;
            this.onSettingsChangedCallback = onSettingsChangedCallback;
            cameraSettingsContainer = root.Q<VisualElement>("settings-window-fields");
            showPanelButton.RegisterCallback<ClickEvent>(OnClickShowPanelButton);

            fovSlider = root.Q<Slider>("fov-slider");
            fovField = root.Q<FloatField>("fov-field");
            nearClipPlane = root.Q<FloatField>("nearclip-field");
            farClipPlane = root.Q<FloatField>("farclip-field");

            fovSlider.lowValue = float.Epsilon;
            fovSlider.highValue = 180f;

            fovSlider.RegisterValueChangedCallback(OnFovSliderValueChanged);
            fovField.RegisterValueChangedCallback(OnFovFieldValueChanged);
            nearClipPlane.RegisterValueChangedCallback(OnNearClipPlaneValueChanged);
            farClipPlane.RegisterValueChangedCallback(OnFarClipPlaneValueChanged);

            root.style.display = DisplayStyle.None;
            showPanelButton.EnableInClassList(ACTIVE_BUTTON_CLASS, false);
        }

        private void OnClickClosePanelButton(ClickEvent evt)
        {
            SetActive(false);
        }

        private void OnFarClipPlaneValueChanged(ChangeEvent<float> evt)
        {
            onSettingsChangedCallback?.Invoke(GetCameraSpotSettings());
        }

        private void OnNearClipPlaneValueChanged(ChangeEvent<float> evt)
        {
            if (evt.newValue < 0.01f)
                nearClipPlane.SetValueWithoutNotify(0.01f);
            onSettingsChangedCallback?.Invoke(GetCameraSpotSettings());
        }

        private void OnFovFieldValueChanged(ChangeEvent<float> evt)
        {
            fovSlider.SetValueWithoutNotify(evt.newValue);
            onSettingsChangedCallback?.Invoke(GetCameraSpotSettings());
        }

        private void OnFovSliderValueChanged(ChangeEvent<float> evt)
        {
            fovField.SetValueWithoutNotify(evt.newValue);
            onSettingsChangedCallback?.Invoke(GetCameraSpotSettings());
        }

        private void OnClickShowPanelButton(ClickEvent evt)
        {
            TogglePanel();
        }

        private void TogglePanel()
        {
            SetActive(!isActive);
        }

        private void SetActive(bool active)
        {
            root.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
            showPanelButton.EnableInClassList(ACTIVE_BUTTON_CLASS, active);
        }

        public CameraSpotSettings GetCameraSpotSettings()
        {
            return new CameraSpotSettings(fovSlider.value, nearClipPlane.value, farClipPlane.value);
        }

        public void SetCameraSpotSettings(CameraSpotSettings settings)
        {
            if (settings == null) return;
            fovSlider.SetValueWithoutNotify(settings.fov);
            fovField.SetValueWithoutNotify(settings.fov);
            nearClipPlane.SetValueWithoutNotify(settings.nearClipPlane);
            farClipPlane.SetValueWithoutNotify(settings.farClipPlane);
        }
    }
}
