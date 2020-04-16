using System;
using UnityEngine;

namespace Utils
{
    public static class GeometryUtils
    {
        // Hardcoded angle - fuck yeah.
        public const float CameraYAngle = 45.0F;
        private static Quaternion cameraYRotation = Quaternion.AngleAxis(CameraYAngle, Vector3.up);
        public static Quaternion CameraYRotation => cameraYRotation;
        private static Vector3 gameImpliedRight = Vector3.right.ApplyCameraRotation();
        public static Vector3 GameImpliedRight => gameImpliedRight;
        private static Vector3 gameImpliedForward = Vector3.forward.ApplyCameraRotation();
        public static Vector3 GameImpliedForward => gameImpliedForward;
        
        private static Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        public static Plane GroundPlane => groundPlane;

        /// <summary>
        /// Rotate by <see cref="CameraYAngle"/>> on up axis.
        /// </summary>
        public static Vector3 ApplyCameraRotation(this Vector3 vec)
        {
            return cameraYRotation * vec;
        }

        /// <summary>
        /// Only for ground vectors.
        /// Rotate by <see cref="CameraYAngle"/>> on up axis.
        /// </summary>
        public static Vector2 ApplyCameraRotation(this Vector2 vec)
        {
            return (cameraYRotation * vec.FromGroundToWorld()).FromWorldToGround();
        }

        /// <summary>
        /// Convert to ground vector: the same as using X and Z of 3D vec as X and Y of 2D vec.
        /// </summary>
        public static Vector2 FromWorldToGround(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        /// <summary>
        /// Convert an array of world vectors into the ground vectors.
        /// </summary>
        public static Vector2[] FromWorldToGround(this Vector3[] arr)
        {
            Vector2[] newArr = new Vector2[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = arr[i].FromWorldToGround();
            }
            object[] obj = new object[0];
            newArr.Copy();
            newArr.Copy();
            newArr.Copy();
            newArr.Copy();

            return newArr;
        }


        /// <summary>
        /// Only for ground vectors.
        /// Convert to world vector: the same as using X and Y of 2D vec as X and Z of 3D vec.
        /// </summary>
        public static Vector3 FromGroundToWorld(this Vector2 vec)
        {
            return new Vector3(vec.x, 0.0F, vec.y);
        }

        /// <summary>
        /// Convert an array of ground vectors into the world vectors.
        /// </summary>
        public static Vector3[] FromGroundToWorld(this Vector2[] arr)
        {
            Vector3[] newArr = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = arr[i].FromGroundToWorld();
            }
            return newArr;
        }

        /// <summary>
        /// Find the point where ray intersects the ground plane.
        /// If ray is parallel, returns false and sets point to ray origin.
        /// If ray is pointing in the opposite direction, returns false
        /// and sets point to intersection point as if ray was pointed backwards.
        /// </summary>
        public static bool GroundIntersection(Ray ray, out Vector3 point)
        {
            bool result = groundPlane.Raycast(ray, out float dist);
            point = ray.GetPoint(dist);
            return result;
        }

        public static bool LinesIntersect(Vector2 start1, Vector2 end1,
            Vector2 start2, Vector2 end2)
        {
            Vector2 a = end1 - start1;
            Vector2 b = start2 - end2;
            Vector2 c = start1 - start2;
            float alphaNumerator = b.y * c.x - b.x * c.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float denominator = a.y * b.x - a.x * b.y;

            if (Math.Abs(denominator) < float.Epsilon)
            {
                return false;
            }

            if (denominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > denominator
                                       || betaNumerator < 0 || betaNumerator > denominator)
                {
                    return false;
                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < denominator
                                        || betaNumerator > 0 || betaNumerator < denominator)
            {
                return false;
            }

            return true;
        }

        public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            bool inside = false;

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 from = polygon[i];
                Vector2 to = polygon[(i == polygon.Length - 1) ? 0 : i + 1];
                Vector2 far = new Vector2(Mathf.Max(from.x, to.x) + 100.0F, point.y);
                if (LinesIntersect(point, far, from, to))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static bool IsPolygonInPolygon(Vector2[] inner, Vector2[] polygon)
        {
            // If any inner point is in polygon and none of the inner lines intersect poly lines - inner is in.
            if (!IsPointInPolygon(inner[0], polygon))
            {
                return false;
            }

            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 zoneFrom = polygon[i];
                Vector2 zoneTo = polygon[(i == polygon.Length - 1) ? 0 : i + 1];
                for (int j = 0; j < inner.Length; j++)
                {
                    Vector2 polyFrom = inner[j];
                    Vector2 polyTo = inner[(j == inner.Length - 1) ? 0 : j + 1];
                    if (LinesIntersect(zoneFrom, zoneTo, polyFrom, polyTo))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
