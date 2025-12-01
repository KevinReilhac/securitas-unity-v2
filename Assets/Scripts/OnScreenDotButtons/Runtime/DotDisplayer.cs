using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scope.OnScreenDotButtons
{
    public class DotDisplayer : Manager<DotDisplayer>
    {
        [SerializeField] private Transform dotsParent;
        [SerializeField] private Camera cam;

        private List<Dot> dots = new List<Dot>();

        public Camera Cam
        {
            get
            {
                if (cam == null)
                    cam = Camera.main;
                return cam;
            }
        }

        public void AddDot(string text, Transform target, Action onClick)
        {
            Dot dot = CreateDot(text, target, onClick);
            dot.transform.localScale = Vector3.one * DotSettings.Instance.DotScale;
            dot.Setup(text, target, onClick);
            dots.Add(dot);
        }

        private Dot CreateDot(string text, Transform target, Action onClick)
        {
            return Instantiate(DotSettings.Instance.DotPrefab, dotsParent);
        }

        public void RemoveDot(Dot dot)
        {
            GameObject.Destroy(dot.gameObject);
            dots.Remove(dot);
        }

        public void ClearDots()
        {
            foreach (Dot dot in dots)
                GameObject.Destroy(dot.gameObject);
            dots.Clear();
        }

        private void Update()
        {
            UpdateDotPositions();
        }

        private void UpdateDotPositions()
        {
            foreach (Dot dot in dots)
                dot.UpdatePosition();
        }
    }
}