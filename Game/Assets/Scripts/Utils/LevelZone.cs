using System;
using log4net;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Convenient utility for managing in-world zones. Use handles to move points around,
    /// and inspector to duplicate, remove or add points.
    /// </summary>
    public class LevelZone : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LevelZone));

        [SerializeField]
        private Vector2[] points =
        {
            new Vector2(-0.5F, -0.5F),
            new Vector2(0, 0.5F),
            new Vector2(0.5F, -0.5F)
        };

        public Vector2[] Points
        {
            get
            {
                Vector2[] copy = new Vector2[points.Length];
                Array.Copy(points, copy, points.Length);
                return copy;
            }
            set
            {
                CheckMinSize(value.Length);
                points = new Vector2[value.Length];
                Array.Copy(value, points, value.Length);
            }
        }

        public Vector3[] GetPointsInWorld()
        {
            Vector3 offset = transform.position;

            Vector3[] pointsInWorld = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];
                pointsInWorld[i] = new Vector3(point.x, 0.0F, point.y) + offset;
            }

            return pointsInWorld;
        }

        public void SetPointsInWorld(Vector3[] pointsInWorld)
        {
            CheckMinSize(pointsInWorld.Length);

            Vector3 offset = transform.position;

            points = new Vector2[pointsInWorld.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = pointsInWorld[i] - offset;
                points[i] = new Vector2(point.x, point.z);
            }
        }

        public int PointCount()
        {
            return points.Length;
        }

        public bool IsPointInZone(Vector2 point)
        {
            return GeometryUtils.IsPointInPolygon(point, points);
        }

        public bool IsWorldPointInZone(Vector3 worldPoint)
        {
            Vector3 point = worldPoint - transform.position;
            return IsPointInZone(new Vector2(point.x, point.z));
        }

        public bool IsPolygonInZone(Vector2[] polyPoints)
        {
            return GeometryUtils.IsPolygonInPolygon(polyPoints, points);
        }

        public bool IsWorldPolygonInZone(Vector3[] worldPolyPoints)
        {
            Vector3 offset = transform.position;
            Vector2[] polyPoints = new Vector2[worldPolyPoints.Length];
            for (int i = 0; i < worldPolyPoints.Length; i++)
            {
                Vector3 point = worldPolyPoints[i] - offset;
                polyPoints[i] = new Vector2(point.x, point.z);
            }

            return GeometryUtils.IsPolygonInPolygon(polyPoints, points);
        }


        private void OnValidate()
        {
            try
            {
                CheckMinSize(points.Length);
            }
            catch (ArgumentException)
            {
                Vector2[] temp = points;
                points = new Vector2[3];
                Array.Copy(temp, points, temp.Length);
                throw;
            }
        }

        private void CheckMinSize(int size)
        {
            if (size < 3)
            {
                throw new ArgumentException("Zone can't have less than 3 points");
            }
        }
    }
}
