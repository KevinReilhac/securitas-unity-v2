using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spop.AreaSystem.UI
{
    public class AreaPanelLine : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI text;

        private Action onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        public void Setup(string text, Action onClick)
        {
            this.text.text = text;
            this.onClick = onClick;
        }
    }
}
