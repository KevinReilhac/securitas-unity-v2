using UnityEngine;

namespace Spop.CameraSystem
{
    [System.Serializable]
    public class OrbitalCameraSpotData
    {
        public const float DEFAULT_MINMAX_H_ANGLE = 360f;
        public const float DEFAULT_MINMAX_V_ANGLE = 180f;
        [Header("Horizontal")]
        [SerializeField] private bool useVerticalAngle = true;
        [SerializeField] private Range verticalAngle = new Range(-20, 20);
        [Header("Vertical")]
        [SerializeField] private bool useHorizontalAngle = true;
        [SerializeField] private Range horizontalAngle = new Range(-80, 80);
        [Header("Distance")]
        [SerializeField][Min(0f)] private float minDistance = 0.5f;
        [SerializeField][Min(0f)] private float distanceThikness = 1f;

        [Header("Start position")]
        [SerializeField] [Range(0f, 1f)] private float startPosXNormalized = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float startPosYNormalized = 0.5f;
        [Space]
        [SerializeField] [Range(0f, 1f)] private float startPosDistNormalized = 0.5f;

        public bool UseVerticalAngle
        {
            get => useVerticalAngle;
            set => useVerticalAngle = value;
        }

        public bool UseHorizontalAngle
        {
            get => useHorizontalAngle;
            set => useHorizontalAngle = value;
        }

        public float HorizontalMax
        {
            get => useHorizontalAngle ? horizontalAngle.max : 360;
            set
            {
                if (useHorizontalAngle)
                    horizontalAngle.max = value;
            }
        }

        public float HorizontalMin
        {
            get => useHorizontalAngle ? horizontalAngle.min : -360;
            set
            {
                if (useHorizontalAngle)
                    horizontalAngle.min = value;
            }
        }

        public float HorizontalMinValue
        {
            get => horizontalAngle.min;
        }

        public float HorizontalMaxValue
        {
            get => horizontalAngle.max;
        }

        public float VerticalMinValue
        {
            get => verticalAngle.min;
        }

        public float VerticalMaxValue
        {
            get => verticalAngle.max;
        }

        public float VerticalMax
        {
            get => useVerticalAngle ? verticalAngle.max : 180;
            set
            {
                if (useVerticalAngle)
                    verticalAngle.max = value;
            }
        }

        public float VerticalMin
        {
            get => useVerticalAngle ? verticalAngle.min : -180;
            set
            {
                if (useVerticalAngle)
                    verticalAngle.min = value;
            }
        }

        public float StartPosXNormalized
        {
            get => startPosXNormalized;
            set => startPosXNormalized = value;
        }
        

        public float StartPosYNormalized
        {
            get => startPosYNormalized;
            set => startPosYNormalized = value;
        }

        public float StartDistanceNormalized
        {
            get => startPosDistNormalized;
            set => startPosDistNormalized = value;
        }

        public float StartPosX
        {
            get => Mathf.Lerp(HorizontalMin, HorizontalMax, startPosXNormalized);
            set => startPosXNormalized = Mathf.InverseLerp(HorizontalMin, HorizontalMax, value);
        }
        public float StartPosY
        {
            get => Mathf.Lerp(VerticalMin, VerticalMax, startPosYNormalized);
            set => startPosYNormalized = Mathf.InverseLerp(VerticalMin, VerticalMax, value);
        }

        public float MinDistance
        {
            get => minDistance;
            set => minDistance = value;
        }
        public float DistanceThickness
        {
            get => distanceThikness;
            set => distanceThikness = value;
        }
        public float MaxDistance
        {
            get => minDistance + distanceThikness;
            set => distanceThikness = value - minDistance;
        }
        public float StartDistance => Mathf.Lerp(MinDistance, MaxDistance, startPosDistNormalized);
        public float StartHorizontal => Mathf.Lerp(HorizontalMin, HorizontalMax, startPosXNormalized);
        public float StartVertical => Mathf.Lerp(VerticalMin, VerticalMax, startPosYNormalized);

        /// <summary>
        /// Get the start position of the camera.
        /// </summary>
        /// <param name="transform">The transform of the camera.</param>
        /// <returns>The start position of the camera.</returns>
        public Vector3 GetCameraStartPosition(Transform transform)
        {
            Quaternion rot = Quaternion.Euler(StartVertical, StartHorizontal, 0);
            return rot * new Vector3(0.0f, 0.0f, -StartDistance) + transform.position;
        }

        /// <summary>
        /// Get the position of the camera.
        /// </summary>
        /// <param name="transform">The transform of the camera.</param>
        /// <param name="pitch">The pitch of the camera.</param>
        /// <param name="yaw">The yaw of the camera.</param>
        public Vector3 GetCameraPosition(Transform transform, float pitch, float yaw, float normalizedDistance)
        {
            float distance = Mathf.Lerp(MinDistance, MaxDistance, normalizedDistance);
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            Vector3 position = rot * new Vector3(0.0f, 0.0f, -distance) + transform.position;
            return position;
        }

        public Vector3 GetCameraPositionNormalized(Transform transform, float normalizedVertical, float normalizedHorizontal, float normalizedDistance)
        {
            float vertical = Mathf.Lerp(VerticalMin, VerticalMax, normalizedVertical);
            float horizontal = Mathf.Lerp(HorizontalMin, HorizontalMax, normalizedHorizontal);
            float distance = Mathf.Lerp(MinDistance, MaxDistance, normalizedDistance);

            Quaternion rot = Quaternion.Euler(vertical, horizontal, 0);
            return rot * new Vector3(0.0f, 0.0f, -distance) + transform.position;
        }

        /// <summary>
        /// Clamp or loop the pitch and yaw.
        /// </summary>
        /// <param name="v">The pitch of the camera.</param>
        /// <param name="h">The yaw of the camera.</param>
        public void ClampOrLoopPitchAndYaw(ref float v, ref float h)
        {
            v = Mathf.Clamp(v, VerticalMin, VerticalMax);

            if (useHorizontalAngle)
            {
                h = Mathf.Clamp(h, HorizontalMin, HorizontalMax);
            }
            else
            {
                if (h > 360f) h = -360f;
                else if (h < -360) h = 360;
            }
        }

#if UNITY_EDITOR
        private static readonly Color minDistColor = Color.green;
        private static readonly Color maxDistColor = Color.blue;

        public void DrawDebug(Transform target)
        {
            Gizmos.color = new Color(1f, 0f, 0, 0.1f);
            Gizmos.DrawWireSphere(target.position, MinDistance);
            Gizmos.DrawWireSphere(target.position, MaxDistance);
            DrawArc(target.position, target.position - Vector3.forward * MinDistance, Vector3.up, HorizontalMin, HorizontalMax, color: minDistColor);
            DrawArc(target.position, target.position - Vector3.forward * MaxDistance, Vector3.up, HorizontalMin, HorizontalMax, color: maxDistColor);

            DrawArc(target.position, target.position + Vector3.forward * MinDistance, -Vector3.right, VerticalMax, VerticalMin, color: minDistColor);
            DrawArc(target.position, target.position + Vector3.forward * MaxDistance, -Vector3.right, VerticalMax, VerticalMin, color: maxDistColor);

            Gizmos.color = Color.white;
            Gizmos.DrawIcon(GetCameraStartPosition(target), "d_SceneViewCamera@2x", false);
            Gizmos.DrawLine(GetCameraStartPosition(target), target.position);
        }

        static public void DrawArc(Vector3 center, Vector3 point, Vector3 axis, float minAngle = -180f, float maxAngle = 180, int segments = 100, Color color = default)
        {
            segments = Mathf.Max(1, segments);

            float rad1 = Mathf.Deg2Rad * minAngle;
            float rad2 = Mathf.Deg2Rad * maxAngle;
            float delta = rad2 - rad1;

            float fsegs = (float)segments;
            float inv_fsegs = 1f / fsegs;

            Vector3 vdiff = point - center;
            float length = vdiff.magnitude;
            vdiff.Normalize();

            Vector3 prevPoint = point;
            Vector3 nextPoint = Vector3.zero;

            if (Mathf.Abs(rad1) >= 1E-6f) prevPoint = pivotAround(center, axis, vdiff, length, rad1);

            Color oldColor = UnityEditor.Handles.color;
            if (color != default) UnityEditor.Handles.color = color;

            for (float seg = 1f; seg <= fsegs; seg++)
            {
                nextPoint = pivotAround(center, axis, vdiff, length, rad1 + delta * seg * inv_fsegs);
                UnityEditor.Handles.DrawLine(prevPoint, nextPoint, 2f);
                prevPoint = nextPoint;
            }

            UnityEditor.Handles.color = oldColor;
        }

        static Vector3 pivotAround(Vector3 center, Vector3 axis, Vector3 dir, float radius, float radians) => center + radius * (Quaternion.AngleAxis(radians * Mathf.Rad2Deg, axis) * dir);
#endif
    }
}