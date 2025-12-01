using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors
{
    public class CameraDisplayer
    {
        private RenderTexture renderTexture;
        private Camera camera;
        private VisualElement root;

        private ACameraSpot currentCameraSpot;
        
        // Drag state tracking
        private bool isDragging = false;
        private Vector2 lastPointerPosition;

        public CameraDisplayer(RenderTexture renderTexture, VisualElement root)
        {
            this.renderTexture = renderTexture;
            this.root = root;
            CreateCamera();

            root.RegisterCallback<WheelEvent>(OnWheelEvent);
            root.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            root.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            root.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
        }

        private void OnWheelEvent(WheelEvent evt)
        {
            if (currentCameraSpot != null)
            {
                currentCameraSpot.Zoom(-evt.delta.y * Time.deltaTime * 0.2f);
                UpdateCamera();
            }
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            if (currentCameraSpot != null)
            {
                isDragging = true;
                lastPointerPosition = evt.position;
                root.CapturePointer(evt.pointerId);
            }
        }

        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (isDragging && currentCameraSpot != null)
            {
                Vector2 currentPointerPosition = evt.position;
                Vector2 dragDelta = currentPointerPosition - lastPointerPosition;
                if (dragDelta.magnitude > 0.001f) // Only move if there's significant movement
                {
                    currentCameraSpot.EditorMove(dragDelta * Time.deltaTime * 10);
                    UpdateCamera();

                }
                
                lastPointerPosition = currentPointerPosition;
            }
        }

        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            if (isDragging)
            {
                isDragging = false;
                root.ReleasePointer(evt.pointerId);
            }
        }

        public void SetCameraSpot(ACameraSpot cameraSpot)
        {
            if (currentCameraSpot != null)
                currentCameraSpot.OnPositionChanged -= UpdateCamera;
            currentCameraSpot = cameraSpot;
            cameraSpot.OnPositionChanged += UpdateCamera;
            camera.transform.position = cameraSpot.GetStartPosition();
            camera.transform.rotation = cameraSpot.GetStartRotation();
            UpdateCameraSettings(cameraSpot.GetCameraSpotSettings());
        }

        public void UpdateCameraSettings(CameraSpotSettings settings)
        {
            if (camera == null) return;
            camera.fieldOfView = settings.fov;
            camera.nearClipPlane = settings.nearClipPlane;
            camera.farClipPlane = settings.farClipPlane;
        }

        public void UpdateCamera()
        {
            camera.transform.position = currentCameraSpot.GetPosition();
            camera.transform.rotation = currentCameraSpot.GetRotation();
            camera.Render();
            root.MarkDirtyRepaint();
        }

        public void ResetCameraPosition()
        {
            camera.transform.position = currentCameraSpot.GetStartPosition();
            camera.transform.rotation = currentCameraSpot.GetStartRotation();
        }

        public void CreateCamera()
        {
            camera = new GameObject("CameraConfiguratorCamera").AddComponent<Camera>();
            camera.targetTexture = renderTexture;
            
            // Re-register event callbacks in case they were unregistered
            if (root != null)
            {
                root.RegisterCallback<WheelEvent>(OnWheelEvent);
                root.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
                root.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
                root.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
            }
        }

        public void DestroyCamera()
        {
            if (camera != null)
            {
                GameObject.DestroyImmediate(camera.gameObject);
                camera = null;
            }
            
            // Clean up event callbacks
            if (root != null)
            {
                root.UnregisterCallback<WheelEvent>(OnWheelEvent);
                root.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
                root.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
                root.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent);
            }
            
            // Reset drag state
            isDragging = false;
        }
    }
}
