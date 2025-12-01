using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors
{
    public interface ICameraFields
    {
        Type GetCameraSpotType();

        void SetVisible(bool visible);
        void SetCameraSpot(ACameraSpot cameraSpot);
        void RegisterOnAnyFieldChanged(Action callback);
        void UnregisterOnAnyFieldChanged(Action callback);
        void SetFieldsFromCameraSpot();
    }

    public abstract class baseCameraFields<T> : ICameraFields where T : ACameraSpot
    {
        protected T cameraSpot;
        protected VisualElement root;
        private Action onAnyFieldChanged;

        public baseCameraFields(VisualElement root)
        {
            this.root = root;
            GetFields();
        }

        protected abstract void GetFields();
        public abstract void SetFieldsFromCameraSpot();
        protected void SetCameraSpot(T cameraSpot)
        {
            this.cameraSpot = cameraSpot;
            Debug.Log($"SetCameraSpot: {cameraSpot.name}");
            SetFieldsFromCameraSpot();
        }

        protected void SetDirty()
        {
            onAnyFieldChanged?.Invoke();
            EditorUtility.SetDirty(cameraSpot);
        }

        public Type GetCameraSpotType() => typeof(T);

        public void SetVisible(bool visible)
        {
            root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetCameraSpot(ACameraSpot cameraSpot)
        {
            SetCameraSpot((T)cameraSpot);
        }

        public void RegisterOnAnyFieldChanged(Action callback)
        {
            onAnyFieldChanged += callback;
        }

        public void UnregisterOnAnyFieldChanged(Action callback)
        {
            onAnyFieldChanged -= callback;
        }
    }
}
