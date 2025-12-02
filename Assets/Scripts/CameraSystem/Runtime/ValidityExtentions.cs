using NUnit.Framework;
using UnityEngine;

namespace Spop.CameraSystem
{
    public static class ValidityExtentions
    {
        public static bool IsValid(this Vector3 vector3)
        {
            if (float.IsNaN(vector3.x) || !float.IsFinite(vector3.x))
                return false;
            if (float.IsNaN(vector3.y) || !float.IsFinite(vector3.y))
                return false;
            if (float.IsNaN(vector3.z) || !float.IsFinite(vector3.z))
                return false;
            return true;
        }

        public static bool IsValid(this Vector2 vector2)
        {
            if (float.IsNaN(vector2.x) || !float.IsFinite(vector2.x))
                return false;
            if (float.IsNaN(vector2.y) || !float.IsFinite(vector2.y))
                return false;
            return true;
        }

        public static bool IsValid(this Quaternion quaternion)
        {
            if (float.IsNaN(quaternion.x) || !float.IsFinite(quaternion.x))
                return false;
            if (float.IsNaN(quaternion.y) || !float.IsFinite(quaternion.y))
                return false;
            if (float.IsNaN(quaternion.z) || !float.IsFinite(quaternion.z))
                return false;
            if (float.IsNaN(quaternion.w) || !float.IsFinite(quaternion.w))
                return false;
            return true;
        }
    }
}
