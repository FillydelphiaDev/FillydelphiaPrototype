using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomEditor(typeof(LevelZone)), CanEditMultipleObjects]
    public class LevelZoneEditor : UnityEditor.Editor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LevelZoneEditor));

        private const float PointCapSize = 1.0F;
        private const float MiddleCapSize = 0.5F;
        private const float LineWidth = 10.0F;

        private int currentPointControl;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Click on middle point to split a line\n" +
                                    "Click and drag on main point to move\n" +
                                    "Click + Control on main point to delete", MessageType.Info);
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            try
            {
                HandleZone(target as LevelZone);
            }
            catch (Exception e)
            {
                Log.Error()?.Call("Exception when drawing zone handles", e);
            }
        }

        private void HandleZone(LevelZone zone)
        {
            PolyLine(zone);
            MiddleCaps(zone);
            PointCaps(zone);
            PointLabels(zone);
        }

        private void PolyLine(LevelZone zone)
        {
            Vector3[] points = zone.GetPointsInWorld();
            Handles.color = Color.red;
            Vector3[] pointsPolyLine = new Vector3[points.Length + 1];
            Array.Copy(points, pointsPolyLine, points.Length);
            pointsPolyLine[pointsPolyLine.Length - 1] = pointsPolyLine[0];
            Handles.DrawAAPolyLine(LineWidth, pointsPolyLine);
        }

        private void MiddleCaps(LevelZone zone)
        {
            Vector3[] points = zone.GetPointsInWorld();

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 from = points[i];
                Vector3 to = points[(i == points.Length - 1) ? 0 : i + 1];
                Vector3 middle = (to - from) / 2 + from;

                int controlId = GUIUtility.GetControlID(FocusType.Passive);
                switch (Event.current.GetTypeForControl(controlId))
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(controlId,
                            HandleUtility.DistanceToCircle(middle, MiddleCapSize));
                        break;
                    case EventType.MouseUp:
                        // Split line when left click on the middle
                        Handles.SphereHandleCap(controlId, middle, Quaternion.identity,
                            MiddleCapSize,
                            EventType.Layout);
                        if (HandleUtility.nearestControl == controlId && Event.current.button == 0)
                        {
                            Log.Debug()?.Call($"Line split (index: {i})");
                            List<Vector3> newPoints = new List<Vector3>(points);
                            if (i == points.Length - 1)
                            {
                                newPoints.Insert(0, middle);
                            }
                            else
                            {
                                newPoints.Insert(i + 1, middle);
                            }
                            zone.SetPointsInWorld(newPoints.ToArray());
                            Event.current.Use();
                        }
                        break;
                    case EventType.Repaint:
                        Handles.color = Color.cyan;
                        Handles.SphereHandleCap(controlId, middle, Quaternion.identity,
                            MiddleCapSize,
                            EventType.Repaint);
                        break;
                }
            }
        }

        private void PointCaps(LevelZone zone)
        {
            List<Vector3> points = zone.GetPointsInWorld().ToList();

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 point = points[i];

                int controlId = GUIUtility.GetControlID(FocusType.Passive);
                switch (Event.current.GetTypeForControl(controlId))
                {
                    case EventType.Layout:
                        HandleUtility.AddControl(controlId,
                            HandleUtility.DistanceToCircle(point, PointCapSize));
                        break;
                    case EventType.MouseUp:
                        if (currentPointControl == controlId)
                        {
                            currentPointControl = 0;
                            Log.Debug()?.Call($"Point unlocked (control: {currentPointControl})");
                        }
                        // Remove point when left click + control
                        Handles.SphereHandleCap(controlId, point, Quaternion.identity, PointCapSize,
                            EventType.Layout);
                        if (HandleUtility.nearestControl == controlId &&
                            Event.current.button == 0 && Event.current.control)
                        {
                            Log.Debug()?.Call($"Point deleted (index: {i})");
                            points.RemoveAt(i);
                            i--;
                            Event.current.Use();
                        }
                        break;
                    case EventType.MouseDown:
                        Handles.SphereHandleCap(controlId, point, Quaternion.identity, PointCapSize,
                            EventType.Layout);
                        if (HandleUtility.nearestControl == controlId)
                        {
                            currentPointControl = controlId;
                            Log.Debug()?.Call($"Point locked (index: {i}, " +
                                              $"control: {currentPointControl})");
                            Event.current.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        // Move point on XZ plane
                        if (currentPointControl == controlId)
                        {
                            Vector2 delta = Event.current.delta;
                            Vector2 mousePos = Event.current.mousePosition;
                            Log.Debug()?.Call($"Point moved (index: {i}, " +
                                              $"delta: {delta}, mousePos: {mousePos})");
                            
                            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
                            if (GeometryUtils.GroundIntersection(ray, out Vector3 intersection))
                            {
                                points[i] = intersection;
                            }
                            Event.current.Use();
                        }
                        break;
                    case EventType.Repaint:
                        Handles.color = Color.magenta;
                        Handles.SphereHandleCap(controlId, point, Quaternion.identity, PointCapSize,
                            EventType.Repaint);
                        break;
                }
            }

            zone.SetPointsInWorld(points.ToArray());
        }

        private void PointLabels(LevelZone zone)
        {
            switch (Event.current.type)
            {
                case EventType.Repaint:
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    Vector3[] points = zone.GetPointsInWorld();
                    for (int i = 0; i < points.Length; i++)
                    {
                        Vector3 point = points[i];
                        Vector3 labelPos = point;
                        labelPos.y += 1.0F;
                        Handles.Label(labelPos, i.ToString(), style);
                    }
                    break;
            }
        }
    }
}
