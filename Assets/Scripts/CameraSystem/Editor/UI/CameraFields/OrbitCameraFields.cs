using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors
{
    public class OrbitCameraFields : baseCameraFields<OrbitalCameraSpot>
    {
        private Slider startposXSlider;
        private Slider startposYSlider;
        private Slider startposDistSlider;
        private FloatField distanceMinField;
        private FloatField distanceThicknessField;
        private MinMaxSlider horizontalAngleField;
        private Toggle horizontalMinMaxEnabled;
        private MinMaxSlider verticalAngleField;
        private Toggle verticalMinMaxEnabled;


        public OrbitCameraFields(VisualElement root) : base(root)
        {
        }

        protected override void GetFields()
        {
            startposXSlider = root.Q<Slider>("start-x-position");
            startposYSlider = root.Q<Slider>("start-y-position");
            startposDistSlider = root.Q<Slider>("start-dist-position");
            distanceMinField = root.Q<FloatField>("distance-min");
            distanceThicknessField = root.Q<FloatField>("distance-thickness");
            horizontalAngleField = root.Q<MinMaxSlider>("horizontal-min-max");
            horizontalMinMaxEnabled = root.Q<Toggle>("horizontal-min-max-enabled");
            verticalAngleField = root.Q<MinMaxSlider>("vertical-min-max");
            verticalMinMaxEnabled = root.Q<Toggle>("vertical-min-max-enabled");

            startposXSlider.lowValue = 0;
            startposXSlider.highValue = 1;
            startposYSlider.lowValue = 0;
            startposYSlider.highValue = 1;
            startposDistSlider.lowValue = 0;
            startposDistSlider.highValue = 1;
            horizontalAngleField.lowLimit = -360;
            horizontalAngleField.highLimit = 360;
            verticalAngleField.lowLimit = -89.9f;
            verticalAngleField.highLimit = 89.9f;

            root.Q<Button>("set-startpos-button").clicked += OnClickSetStartPosButton;
            root.Q<Button>("snap-startpos-button").clicked += OnClickSnapStartPosButton;
            root.Q<Button>("set-min-dist-button").clicked += OnClickSetMinDistanceButton;
            root.Q<Button>("set-max-dist-button").clicked += OnClickSetMaxDistanceButton;
            root.Q<Button>("set-h-min-button").clicked += OnClickSetHorizontalMinButton;
            root.Q<Button>("set-h-max-button").clicked += OnClickSetHorizontalMaxButton;
            root.Q<Button>("set-v-min-button").clicked += OnClickSetVerticalMinButton;
            root.Q<Button>("set-v-max-button").clicked += OnClickSetVerticalMaxButton;


            Debug.Log($"Setup events");
            startposXSlider.RegisterValueChangedCallback(OnStartPosXChanged);
            startposYSlider.RegisterValueChangedCallback(OnStartPosYChanged);
            startposDistSlider.RegisterValueChangedCallback(OnStartPosDistChanged);
            distanceMinField.RegisterValueChangedCallback(OnDistanceMinChanged);
            distanceThicknessField.RegisterValueChangedCallback(OnDistanceThicknessChanged);
            horizontalAngleField.RegisterValueChangedCallback(OnHorizontalAngleChanged);
            verticalAngleField.RegisterValueChangedCallback(OnVerticalAngleChanged);
            horizontalMinMaxEnabled.RegisterValueChangedCallback(OnHorizontalMinMaxEnabledChanged);
            verticalMinMaxEnabled.RegisterValueChangedCallback(OnVerticalMinMaxEnabledChanged);
        }



        private void OnClickSetStartPosButton()
        {
            Vector2 normalizedPos = cameraSpot.GetControlPositionNormalized();
            Debug.Log($"OnClickSetStartPosButton: {normalizedPos}");
            startposXSlider.value = normalizedPos.x;
            startposYSlider.value = normalizedPos.y;
            startposDistSlider.value = cameraSpot.GetControllDistanceNormalized();
            SetDirty();
        }

        private void OnClickSnapStartPosButton()
        {
            cameraSpot.SetPositionNormalized(new Vector2(startposXSlider.value, startposYSlider.value), startposDistSlider.value);
        }

        private void OnClickSetMinDistanceButton()
        {
            distanceMinField.value = cameraSpot.GetControllDistance();
            SetDirty();
        }

        private void OnClickSetMaxDistanceButton()
        {
            distanceThicknessField.value = cameraSpot.GetControllDistance() - distanceMinField.value;
            SetDirty();
        }

        private void OnClickSetHorizontalMinButton()
        {
            horizontalAngleField.value = new Vector2(cameraSpot.GetControlPosition().x, horizontalAngleField.value.y);
            SetDirty();
        }

        private void OnClickSetHorizontalMaxButton()
        {
            horizontalAngleField.value = new Vector2(horizontalAngleField.value.x, cameraSpot.GetControlPosition().x);
            SetDirty();
        }

        private void OnClickSetVerticalMinButton()
        {
            verticalAngleField.value = new Vector2(cameraSpot.GetControlPosition().y, verticalAngleField.value.y);
            SetDirty();
        }

        private void OnClickSetVerticalMaxButton()
        {
            verticalAngleField.value = new Vector2(verticalAngleField.value.x, cameraSpot.GetControlPosition().y);
            SetDirty();
        }

        #region Field Callbacks
        private void OnVerticalMinMaxEnabledChanged(ChangeEvent<bool> evt)
        {
            cameraSpot.OrbitalData.UseVerticalAngle = evt.newValue;
            SetDirty();
        }

        private void OnHorizontalMinMaxEnabledChanged(ChangeEvent<bool> evt)
        {
            cameraSpot.OrbitalData.UseHorizontalAngle = evt.newValue;
            SetDirty();
        }

        private void OnVerticalAngleChanged(ChangeEvent<Vector2> evt)
        {
            Debug.Log($"OnVerticalAngleChanged: {evt.newValue}");
            cameraSpot.OrbitalData.VerticalMin = evt.newValue.x;
            cameraSpot.OrbitalData.VerticalMax = evt.newValue.y;
            SetDirty();
        }

        private void OnHorizontalAngleChanged(ChangeEvent<Vector2> evt)
        {
            cameraSpot.OrbitalData.HorizontalMin = evt.newValue.x;
            cameraSpot.OrbitalData.HorizontalMax = evt.newValue.y;
            SetDirty();
        }

        private void OnDistanceThicknessChanged(ChangeEvent<float> evt)
        {
            if (evt.newValue < 0)
            {
                distanceThicknessField.value = 0;
                return;
            }
            cameraSpot.OrbitalData.DistanceThickness = evt.newValue;
            SetDirty();
        }

        private void OnDistanceMinChanged(ChangeEvent<float> evt)
        {
            if (evt.newValue < 0)
            {
                distanceMinField.value = 0;
                return;
            }
            cameraSpot.OrbitalData.MinDistance = evt.newValue;
            SetDirty();
        }

        private void OnStartPosYChanged(ChangeEvent<float> evt)
        {
            cameraSpot.OrbitalData.StartPosYNormalized = Mathf.Clamp01(evt.newValue);
            SetDirty();
        }

        private void OnStartPosXChanged(ChangeEvent<float> evt)
        {
            cameraSpot.OrbitalData.StartPosXNormalized = Mathf.Clamp01(evt.newValue);
            SetDirty();
        }

        private void OnStartPosDistChanged(ChangeEvent<float> evt)
        {
            cameraSpot.OrbitalData.StartDistanceNormalized = evt.newValue;
            SetDirty();
        }
        #endregion

        public override void SetFieldsFromCameraSpot()
        {
            startposXSlider.SetValueWithoutNotify(cameraSpot.OrbitalData.StartPosXNormalized);
            startposYSlider.SetValueWithoutNotify(cameraSpot.OrbitalData.StartPosYNormalized);
            startposDistSlider.SetValueWithoutNotify(cameraSpot.OrbitalData.StartDistanceNormalized);
            distanceMinField.SetValueWithoutNotify(cameraSpot.OrbitalData.MinDistance);
            distanceThicknessField.SetValueWithoutNotify(cameraSpot.OrbitalData.DistanceThickness);
            horizontalAngleField.SetValueWithoutNotify(new Vector2(cameraSpot.OrbitalData.HorizontalMinValue, cameraSpot.OrbitalData.HorizontalMaxValue));
            verticalAngleField.SetValueWithoutNotify(new Vector2(cameraSpot.OrbitalData.VerticalMinValue, cameraSpot.OrbitalData.VerticalMaxValue));
            horizontalMinMaxEnabled.SetValueWithoutNotify(cameraSpot.OrbitalData.UseHorizontalAngle);
            verticalMinMaxEnabled.SetValueWithoutNotify(cameraSpot.OrbitalData.UseVerticalAngle);
        }
    }
}
