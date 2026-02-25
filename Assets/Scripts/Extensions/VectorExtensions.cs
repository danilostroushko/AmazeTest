using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static Vector3 AddX(this Vector3 vector, float value)
        {
            vector.x += value;
            return vector;
        }

        public static Vector3 AddY(this Vector3 vector, float value)
        {
            vector.y += value;
            return vector;
        }

        public static Vector3 AddZ(this Vector3 vector, float value)
        {
            vector.z += value;
            return vector;
        }

        public static Vector3 SetX(this Vector3 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            vector.z = value;
            return vector;
        }

        public static Vector2 AddX(this Vector2 vector, float value)
        {
            vector.x += value;
            return vector;
        }

        public static Vector2 AddY(this Vector2 vector, float value)
        {
            vector.y += value;
            return vector;
        }

        public static Vector2 SetX(this Vector2 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector2 SetY(this Vector2 vector, float value)
        {
            vector.y = value;
            return vector;
        }
    }
}