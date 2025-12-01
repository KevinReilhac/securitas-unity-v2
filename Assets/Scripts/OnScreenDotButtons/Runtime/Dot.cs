using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Scope.OnScreenDotButtons
{
    public class Dot : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject visualsContainer;

        private Transform target;
        private Action onClick;

        public void Setup(string text, Transform target, Action onClick)
        {
            this.text.text = text;
            this.target = target;
            this.onClick = onClick;
            button.onClick.AddListener(OnClick);
        }

        void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            onClick?.Invoke();
        }

        public void UpdatePosition()
        {
            visualsContainer.SetActive(GetUIPosition(out Vector3 position));

            if (visualsContainer.activeSelf)
            {
                transform.position = position;
            }
        }

        /// <summary>
        /// Get UI position from selector position
        /// </summary>
        /// <param name="position"></param>
        /// <returns> Is Selector in front of camera </returns>
        private bool GetUIPosition(out Vector3 position)
        {
            if (Camera.main == null)
            {
                position = Vector3.one * -1;
                return false;
            }
            Vector3 screenPoint = DotDisplayer.instance.Cam.WorldToScreenPoint(target.position);

            if (screenPoint.z < 0)
            {
                position = Vector3.one * -1;
                return false;
            }

            position = screenPoint;
            return true;
        }
    }
}