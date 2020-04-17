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

        // Ground vectors
        [SerializeField]
        private Vector2[] points =
        {
            new Vector2(-0.5F, -0.5F),
            new Vector2(0, 0.5F),
            new Vector2(0.5F, -0.5F)
        };

        public Vector2[] Points
        {
            get => points.Copy();
            set
            {
                CheckMinSize(value.Length);
                points = value.Copy();
            }
        }

        public Vector3[] GetPointsInWorld()
        {
            Vector3 offset = transform.position;
            return FromLocalToWorld(points, offset);
        }

        public void SetPointsInWorld(Vector3[] pointsInWorld)
        {
            CheckMinSize(pointsInWorld.Length);

            Vector3 offset = transform.position;
            points = FromWorldToLocal(pointsInWorld, offset);
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
            return IsPointInZone((worldPoint - transform.position).FromWorldToGround());
        }

        public bool IsPolygonInZone(Vector2[] polyPoints)
        {
            return GeometryUtils.IsPolygonInPolygon(polyPoints, points);
        }

        public bool IsWorldPolygonInZone(Vector3[] worldPolyPoints)
        {
            Vector3 offset = transform.position;
            Vector2[] polyPoints = FromWorldToLocal(worldPolyPoints, offset);
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

        private Vector2[] FromWorldToLocal(Vector3[] worldPoints, Vector3 offset)
        {
            Vector2[] localPoints = new Vector2[worldPoints.Length];
            for (int i = 0; i < worldPoints.Length; i++)
            {
                localPoints[i] = (worldPoints[i] - offset).FromWorldToGround();
            }
            return localPoints;
        }

        private Vector3[] FromLocalToWorld(Vector2[] localPoints, Vector3 offset)
        {
            Vector3[] worldPoints = new Vector3[localPoints.Length];
            for (int i = 0; i < localPoints.Length; i++)
            {
                worldPoints[i] = localPoints[i].FromGroundToWorld() + offset;
            }
            return worldPoints;
        } 
    }
}
