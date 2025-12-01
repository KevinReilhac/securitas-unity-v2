using System;
using UnityEngine;
using UnityEngine.UI;

namespace Spop.CameraSystem.Editors
{
    public class MultiPressButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private int pressCount = 5;
        [SerializeField] private GameObject objectToToggle;
        [SerializeField] private float timeoutSeconds = 3.0f;

        private int currentPressCount = 0;
        private float lastPressTime = 0f;

        void Awake()
        {
            objectToToggle.SetActive(false);
        }

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(OnButtonPressed);
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnButtonPressed);
        }

        private void Update()
        {
            // Check if timeout has occurred and reset if needed
            if (currentPressCount > 0 && Time.time - lastPressTime > timeoutSeconds)
            {
                currentPressCount = 0;
            }
        }

        private void OnButtonPressed()
        {
            currentPressCount++;
            lastPressTime = Time.time;

            if (currentPressCount >= pressCount)
            {
                ToggleObject();
                currentPressCount = 0; // Reset counter after toggle
            }
        }

        private void ToggleObject()
        {
            if (objectToToggle != null)
            {
                objectToToggle.SetActive(!objectToToggle.activeSelf);
            }
        }

        // Public method to reset the press count if needed
        public void ResetPressCount()
        {
            currentPressCount = 0;
            lastPressTime = 0f;
        }

        // Public method to manually trigger the toggle
        public void ForceToggle()
        {
            ToggleObject();
            currentPressCount = 0;
        }
    }
}