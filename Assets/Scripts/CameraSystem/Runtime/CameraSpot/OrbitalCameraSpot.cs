using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Spop.CameraSystem
{
    public class OrbitalCameraSpot : ACameraSpot
    {
        [SerializeField] private OrbitalCameraSpotData orbitalData;

        [Header("Controls")]
        [SerializeField] private float xPosition = 0f;
        [SerializeField] private float yPosition = 0f;
        [SerializeField] private float normalizedDistance = 0f;
        [Header("Speed Multiplier")]
        [SerializeField] private float moveSpeedMultiplier = 1f;
        [SerializeField] private float zoomSpeedMultiplier = 1f;

        public float targetXPosition {get; private set;}
        public float targetYPosition {get; private set;}
        private Tween tween;

        public OrbitalCameraSpotData OrbitalData => orbitalData;

        public override Vector3 GetStartPosition()
        {
            return orbitalData.GetCameraStartPosition(transform);
        }

        public override Quaternion GetStartRotation()
        {
            Vector3 direction = transform.position - orbitalData.GetCameraStartPosition(transform);
            return Quaternion.LookRotation(direction);
        }

        public void SetPosition(Vector2 position, float? distance = null)
        {
            xPosition = position.x;
            yPosition = position.y;
            if (distance.HasValue)
                normalizedDistance = Mathf.InverseLerp(orbitalData.MinDistance, orbitalData.MaxDistance, distance.Value);
            OnPositionChanged?.Invoke();
            ClampPosition();
        }

        public void SetPositionNormalized(Vector2 position, float? distance = null)
        {
            xPosition = Mathf.Lerp(orbitalData.HorizontalMin, orbitalData.HorizontalMax, position.x);
            yPosition = Mathf.Lerp(orbitalData.VerticalMin, orbitalData.VerticalMax, position.y);
            if (distance.HasValue)
                normalizedDistance = distance.Value;
            OnPositionChanged?.Invoke();

            ClampPosition();
        }

        public override void ResetPosition()
        {
            xPosition = orbitalData.StartPosX;
            yPosition = orbitalData.StartPosY;
            targetXPosition = xPosition;
            targetYPosition = yPosition;
            normalizedDistance = orbitalData.StartDistanceNormalized;
            OnPositionChanged?.Invoke();
        }

        public Vector2 GetControlPosition()
        {
            return new Vector2(xPosition, yPosition);
        }

        public Vector2 GetControlPositionNormalized()
        {
            return new Vector2(Mathf.InverseLerp(orbitalData.HorizontalMin, orbitalData.HorizontalMax, xPosition), Mathf.InverseLerp(orbitalData.VerticalMin, orbitalData.VerticalMax, yPosition));
        }

        public float GetControllDistanceNormalized()
        {
            return normalizedDistance;
        }

        public float GetControllDistance()
        {
            return Mathf.Lerp(orbitalData.MinDistance, orbitalData.MaxDistance, normalizedDistance);
        }

        public override Vector3 GetPosition()
        {
            ClampPosition();
            return orbitalData.GetCameraPosition(transform, yPosition, xPosition, normalizedDistance);
        }

        public override Quaternion GetRotation()
        {
            Vector3 direction = transform.position - GetPosition();
            if (direction == Vector3.zero || !direction.IsValid())
                return Quaternion.identity;
            return Quaternion.LookRotation(direction);
        }

        public override void Move(Vector2 moveDelta)
        {
            if (!moveDelta.IsValid())
                return;
            targetXPosition += moveDelta.x * moveSpeedMultiplier;
            targetYPosition -= moveDelta.y * moveSpeedMultiplier;

            if (orbitalData.UseHorizontalAngle)
                targetXPosition = Mathf.Clamp(targetXPosition, orbitalData.HorizontalMin, orbitalData.HorizontalMax);
            if (orbitalData.UseVerticalAngle)
                targetYPosition = Mathf.Clamp(targetYPosition, orbitalData.VerticalMin, orbitalData.VerticalMax);
        }

        void Update()
        {
            if (IsActive())
            {
                xPosition = Mathf.Lerp(xPosition, targetXPosition, Time.deltaTime * CameraSystemSettings.Instance.moveLerpSpeed);
                yPosition = Mathf.Lerp(yPosition, targetYPosition, Time.deltaTime * CameraSystemSettings.Instance.moveLerpSpeed);
                OnPositionChanged?.Invoke();
            }
        }

        public override void Zoom(float zoomDelta)
        {
            normalizedDistance -= zoomDelta * zoomSpeedMultiplier;
            normalizedDistance = Mathf.Clamp01(normalizedDistance);
        }
        
        private void ClampPosition()
        {
            if (orbitalData.UseHorizontalAngle)
                xPosition = Mathf.Clamp(xPosition, orbitalData.HorizontalMin, orbitalData.HorizontalMax);
            if (orbitalData.UseVerticalAngle)
                yPosition = Mathf.Clamp(yPosition, orbitalData.VerticalMin, orbitalData.VerticalMax);
        }

        override protected IEnumerator Animation()
        {
            if (orbitalData.UseHorizontalAngle)
            {
                return LeftRightAnimation();
            }
            else
            {
                return AroundAnimation();
            }
        }

        private IEnumerator AroundAnimation()
        {
            while (IsActive())
            {
                Move(new Vector2(CameraSystemSettings.Instance.AnimationSpeedValue * Time.deltaTime, 0));
                yield return null;
            }
        }

        private IEnumerator LeftRightAnimation()
        {
            while (IsActive())
            {
                while(targetXPosition < orbitalData.HorizontalMax)
                {
                    Move(new Vector2(CameraSystemSettings.Instance.AnimationSpeedValue * Time.deltaTime, 0));
                    yield return null;
                }
                yield return new WaitForSeconds(CameraSystemSettings.Instance.AnimationLeftRightDelayValue);
                while(targetXPosition > orbitalData.HorizontalMin)
                {
                    Move(new Vector2(-CameraSystemSettings.Instance.AnimationSpeedValue * Time.deltaTime, 0));
                    yield return null;
                }
                yield return new WaitForSeconds(CameraSystemSettings.Instance.AnimationLeftRightDelayValue);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            orbitalData.DrawDebug(transform);
        }

        public override void EditorMove(Vector2 dragDelta)
        {
            xPosition += dragDelta.x;
            yPosition += dragDelta.y;

            if (orbitalData.UseHorizontalAngle)
                xPosition = Mathf.Clamp(xPosition, orbitalData.HorizontalMin, orbitalData.HorizontalMax);
            if (orbitalData.UseVerticalAngle)
                yPosition = Mathf.Clamp(yPosition, orbitalData.VerticalMin, orbitalData.VerticalMax);
        }

#endif
    }
}
