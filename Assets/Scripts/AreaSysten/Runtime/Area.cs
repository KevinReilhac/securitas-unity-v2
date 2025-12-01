using System;
using System.Collections.Generic;
using Spop.CameraSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Spop.AreaSystem
{
    public class baseArea : MonoBehaviour
    {
        [Serializable]
        public class Events
        {
            [Tooltip("Before the transition, when the area is entered")]
            public UnityEvent onBeforeAreaEnter;
            [Tooltip("After the transition, when the area is entered")]
            public UnityEvent onAfterAreaEnter;
            [Tooltip("Before the transition, when the area is exited (When goind to any other area)")]
            public UnityEvent onBeforeAreaExitSelf;
            [Tooltip("After the transition, when the area is exited (When goind to any other area)")]
            public UnityEvent onAfterAreaExitSelf;
            [Tooltip("Before the transition, when the area is exited (Not called for child areas)")]
            public UnityEvent onBeforeAreaExit;
            [Tooltip("After the transition, when the area is exited (Not called for child areas)")]
            public UnityEvent onAfterAreaExit;
        }

        //Info
        [SerializeField] protected string areaName = "New Area";
        [SerializeField] protected string displayTitle = string.Empty;
        [SerializeField] protected string subtitle = string.Empty;

        //References
        [SerializeField] private ACameraSpot cameraSpot;
        [SerializeField] private Transform customDotPosition;

        //Events
        [SerializeField] private Events events;

        [HideInInspector] public baseArea ParentArea;

        public string AreaName => areaName;
        public ACameraSpot CameraSpot => cameraSpot;
        public string Subtitle => subtitle;
        public Events Callbacks => events;

        public string DisplayTitle => string.IsNullOrEmpty(displayTitle) ? areaName : displayTitle;

        public Transform GetDotPosition()
        {
            return customDotPosition != null ? customDotPosition : transform;
        }

        public bool IsChildOf(baseArea parent)
        {
            int maxDepth = 100;
            baseArea current = this;
            while (current != null && maxDepth > 0)
            {
                if (current == parent)
                    return true;
                current = current.ParentArea;
                maxDepth--;
            }
            if (maxDepth == 0)
            {
                Debug.LogError("Something went wrong, area tree is too deep, max depth reached");
            }
            return false;
        }

        #region Internal Methods

        internal void SetAreaName(string areaName)
        {
            this.areaName = areaName;
        }

        internal void SetCameraSpot(ACameraSpot cameraSpot)
        {
            this.cameraSpot = cameraSpot;
        }
        #endregion

    }
    /// <summary>
    /// Represents an area in the scene.
    /// </summary>
    public class Area : baseArea
    {
        [SerializeField] private List<baseArea> subAreas;
        [SerializeField] private string description = string.Empty;
        [SerializeField] private string subdescription = string.Empty;

        public IReadOnlyList<baseArea> SubAreas => subAreas.AsReadOnly();

        public string Description => description;
        public string Subdescription => subdescription;

        public void Awake()
        {
            SetupSubAreas();
        }

        private void SetupSubAreas()
        {
            foreach (baseArea subArea in subAreas)
            {
                if (subArea == null)
                    continue;

                subArea.ParentArea = this;
            }
        }

        internal void SetSubAreas(List<baseArea> subAreas)
        {
            this.subAreas = subAreas;
        }

        public IReadOnlyList<baseArea> GetSubAreas()
        {
            return subAreas.AsReadOnly();
        }


    }
}