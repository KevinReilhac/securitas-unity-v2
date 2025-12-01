using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spop.AreaSystem.UI
{

    public class InterestPointPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform interestPointInfoContainer;
        [SerializeField] private Image image;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [Header("Prefabs")]
        [SerializeField] private InterestPointLine interestPointDottedLinePrefab;
        [SerializeField] private InterestPointLine interestPointTextPrefab;
        [SerializeField] private InterestPointLine interestPointTitlePrefab;

        private List<InterestPointLine> interestPointLines = new List<InterestPointLine>();
        private float? defaultSpacing = 0f;

        private void Awake()
        {
            DisableTemplateLines();
            defaultSpacing = verticalLayoutGroup.spacing;
        }

        public void SetInterestPoint(InterestPoint interestPoint)
        {
            ClearInterestPointLines();

            image.sprite = interestPoint.InterestPointInfos.image;
            image.gameObject.SetActive(image.sprite != null);
            if (!defaultSpacing.HasValue)
                defaultSpacing = verticalLayoutGroup.spacing;
            verticalLayoutGroup.spacing = defaultSpacing.Value + interestPoint.SpacingOffset;

            foreach (InterestPointInfos.Info line in interestPoint.InterestPointInfos.infos)
            {
                InterestPointLine lineInstance = Instantiate(GetInterestPointLine(line), interestPointInfoContainer);
                lineInstance.SetText(line.text);
                interestPointLines.Add(lineInstance);
            }
        }

        private InterestPointLine GetInterestPointLine(InterestPointInfos.Info line)
        {
            switch (line.type)
            {
                case InterestPointInfos.InfoTextLineType.DotLine:
                    return interestPointDottedLinePrefab;
                default:
                case InterestPointInfos.InfoTextLineType.Text:
                    return interestPointTextPrefab;
                case InterestPointInfos.InfoTextLineType.Title:
                    return interestPointTitlePrefab;
            }
        }

        private void ClearInterestPointLines()
        {
            foreach (InterestPointLine line in interestPointLines)
            {
                Destroy(line.gameObject);
            }

            interestPointLines.Clear();
        }

        private void DisableTemplateLines()
        {
            foreach (Transform child in interestPointInfoContainer)
            {
                child.gameObject.SetActive(false);
            }
        }

    }
}
