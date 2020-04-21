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

        private static Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        public static Plane GroundPlane => groundPlane;

        public static Vector3 ClearY(this Vector3 vec)
        {
            return new Vector3(vec.x, 0.0F, vec.z);
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
