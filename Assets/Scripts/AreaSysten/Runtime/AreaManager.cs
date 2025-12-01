using Scope.OnScreenDotButtons;
using Spop.CameraSystem;
using UnityEngine;

namespace Spop.AreaSystem
{
    /// <summary>
    /// Manages the current area and triggers events when the area changes.
    /// </summary>
    public class AreaManager : Manager<AreaManager>
    {
        public delegate void AreaChangedDelegate(baseArea from, baseArea to);

        [Header("References")]
        public baseArea defaultArea;

        public baseArea CurrentArea { get; private set; }
        public event AreaChangedDelegate OnAreaChanged;

        protected override void Awake()
        {
            base.Awake();

            if (defaultArea == null)
            {
                Debug.LogWarning("Default Area is not set in the AreaManager");
                defaultArea = GameObject.FindAnyObjectByType<baseArea>();
            }
        }

        private void Start()
        {
            SetCurrentArea(defaultArea, false);
        }

        public async void SetCurrentArea(baseArea area, bool animate = true)
        {
            baseArea oldArea = CurrentArea;
            CurrentArea = area;

            if (oldArea != null)
            {
                if (!CurrentArea.IsChildOf(oldArea))
                    oldArea.Callbacks.onBeforeAreaExit.Invoke();
                oldArea.Callbacks.onBeforeAreaExitSelf.Invoke();
            }

            CurrentArea.Callbacks.onBeforeAreaEnter.Invoke();


            UpdateOnScreenDots(CurrentArea);
            OnAreaChanged?.Invoke(oldArea, CurrentArea);

            await CameraManager.instance.SetActiveCameraSpotTask(CurrentArea.CameraSpot, animate);

            CurrentArea.Callbacks.onAfterAreaEnter.Invoke();
            if (oldArea != null)
            {
                oldArea.Callbacks.onAfterAreaExitSelf.Invoke();
                if (!CurrentArea.IsChildOf(oldArea))
                    oldArea.Callbacks.onAfterAreaExit.Invoke();
            }
        }

        private void UpdateOnScreenDots(baseArea baseArea)
        {
            DotDisplayer.instance.ClearDots();

            if (baseArea is Area area)
            {
                foreach (baseArea subArea in area.SubAreas)
                {
                    baseArea tmpArea = subArea;
                    DotDisplayer.instance.AddDot(tmpArea.DisplayTitle, tmpArea.GetDotPosition(), () => SetCurrentArea(tmpArea));
                }
            }
        }
    }
}