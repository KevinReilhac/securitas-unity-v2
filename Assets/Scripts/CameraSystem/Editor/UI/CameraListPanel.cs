using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors.UI
{
    public class CameraListPanel
    {
        private const string SELECTED_BUTTON_CLASS = "selected-camera-button";
        private VisualElement root;
        private VisualElement cameraListButtonsContainer;
        private Action<ACameraSpot> onCameraSpotSelected;

        private ACameraSpot selectedCameraSpot = null;
        private Button lastSelectedButton = null;
        private Dictionary<Button, ACameraSpot> cameraByButtons = new Dictionary<Button, ACameraSpot>();
        private Dictionary<ACameraSpot, Button> buttonByCameraSpots = new Dictionary<ACameraSpot, Button>();

        public CameraListPanel(VisualElement root, Action<ACameraSpot> onCameraSpotSelected)
        {
            this.root = root;
            cameraListButtonsContainer = root.Q<VisualElement>("buttons-container");
            this.onCameraSpotSelected = onCameraSpotSelected;

            RefreshCameraList();
            root.Q<Button>("refresh-button").clicked += RefreshCameraList;
        }

        public void RefreshCameraList()
        {
            ACameraSpot[] cameraSpots = GameObject.FindObjectsByType<ACameraSpot>(FindObjectsSortMode.None);
            Array.Sort(cameraSpots, (a, b) => a.name.CompareTo(b.name));

            ClearCameraList();
            foreach (ACameraSpot cameraSpot in cameraSpots)
                AddCameraSpotButton(cameraSpot);
        }

        private void ClearCameraList()
        {
            cameraListButtonsContainer.Clear();
            cameraByButtons.Clear();
            buttonByCameraSpots.Clear();
        }

        private void AddCameraSpotButton(ACameraSpot cameraSpot)
        {
            Button button = new Button();
            button.text = cameraSpot.name;
            button.clicked += () => OnCameraSpotButtonClicked(button);
            cameraListButtonsContainer.Add(button);
            cameraByButtons.Add(button, cameraSpot);
            buttonByCameraSpots.Add(cameraSpot, button);
            if (cameraSpot == selectedCameraSpot)
                button.AddToClassList(SELECTED_BUTTON_CLASS);
        }

        private void OnCameraSpotButtonClicked(Button button)
        {
            if (lastSelectedButton != null)
                lastSelectedButton.RemoveFromClassList(SELECTED_BUTTON_CLASS);
            if (cameraByButtons.TryGetValue(button, out ACameraSpot cameraSpot))
            {
                OnCameraSpotSelected(cameraSpot);
                lastSelectedButton = button;
                selectedCameraSpot = cameraSpot;
                button.AddToClassList(SELECTED_BUTTON_CLASS);
            }
        }

        private void OnCameraSpotSelected(ACameraSpot cameraSpot)
        {
            onCameraSpotSelected?.Invoke(cameraSpot);
        }

        public bool IsCameraSpotSelected(ACameraSpot cameraSpot)
        {
            return selectedCameraSpot == cameraSpot;
        }

        public void SelectCameraSpot(ACameraSpot cameraSpot)
        {
            if (buttonByCameraSpots.TryGetValue(cameraSpot, out Button button))
            {
                OnCameraSpotButtonClicked(button);
            }
        }
    }
}
