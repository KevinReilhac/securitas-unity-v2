using TMPro;
using UnityEngine;

namespace Spop.AreaSystem.UI
{
    public class InterestPointLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}
