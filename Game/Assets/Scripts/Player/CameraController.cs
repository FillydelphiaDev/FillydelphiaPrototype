using log4net;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CameraController));

        private const float ViewZoneAdjustStep = 2.0F;
        private const float ViewZoneAdjustError = 0.01F;

        [Tooltip("Offset object should not be moved.")]
        [SerializeField]
        private GameObject offsetObject = null;

        [SerializeField]
        private GameObject player = null;

        [Tooltip("Allowed view zone. If not set, view is not restricted.")]
        [SerializeField]
        private LevelZone viewZone = null;

        public LevelZone ViewZone
        {
            get => viewZone;
            set => viewZone = value;
        }

        [Header("Settings")]
        [Tooltip("Horizontal size of camera's viewport. Zoom will be adjusted to fit this size.")]
        [SerializeField]
        private float horizontalViewSize = 30.0F;

        [Tooltip("A minimal distance of a player from the view zone edges.")]
        [SerializeField]
        private Vector2 minPlayerDistance = new Vector2(2.0F, 1.0F);

        [Tooltip("Speed of camera adjusting for mouse pos. Time is current distance " +
                 "and value is max adjusted distance per frame.")]
        [SerializeField]
        private AnimationCurve adjustingSpeed = AnimationCurve.EaseInOut(0.0F, 0.02F, 1.0F, 0.1F);

        [SerializeField]
        private float cameraRotation = 45.0F;

        private Camera controlled;

        private GameInput gameInput;

        // Normalized -1 to -1
        private Vector2 cursorPos;
        private Vector2 screenPos;

        private void OnValidate()
        {
            Assert.IsNotNull(offsetObject, "Pivot object should be set");
            Assert.IsNotNull(player, "Player object should be set");
        }

        private void Awake()
        {
            gameInput = new GameInput();
            gameInput.Player.CursorPosition.performed += OnCursor;
        }

        private void Start()
        {
            controlled = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            gameInput.Player.CursorPosition.Enable();
        }

        private void Update()
        {
            AdjustCameraZoom();
            CalcNewScreenPos();

            Vector3 worldOffset = CalcWorldOffset();
            AdjustForViewZone(ref worldOffset);
            offsetObject.transform.localPosition = worldOffset;
        }

        private void AdjustCameraZoom()
        {
            float aspect = controlled.pixelWidth / (float) controlled.pixelHeight;
            controlled.orthographicSize = horizontalViewSize / aspect / 2.0F;
        }

        private void CalcNewScreenPos()
        {
            Vector2 difference = cursorPos - screenPos;
            float distance = difference.magnitude;
            if (distance <= float.Epsilon)
            {
                return;
            }
            
            // Div dist by 2 to map from [-1, 1] to [0, 1]
            // Yes, it's intended that curve is evaluated for dist, not for actual difference
            float distMaxAllowed = adjustingSpeed.Evaluate(distance / 2.0F);
            Vector2 delta = difference.normalized * Mathf.Min(distance, distMaxAllowed);
            screenPos += delta;
        }

        private Vector3 CalcMaxOffset()
        {
            Vector3 start = GeometryUtils
                .GroundAndRayIntersection(controlled.ViewportPointToRay(Vector3.zero))
                .GetValueOrDefault();
            Vector3 intersectionX = GeometryUtils
                .GroundAndRayIntersection(controlled.ViewportPointToRay(new Vector3(1.0F, 0.0F)))
                .GetValueOrDefault();
            Vector3 intersectionY = GeometryUtils
                .GroundAndRayIntersection(controlled.ViewportPointToRay(new Vector3(0.0F, 1.0F)))
                .GetValueOrDefault();

            if (Log.IsDebugEnabled)
            {
                Debug.DrawLine(start, intersectionX, Color.red);
                Debug.DrawLine(start, intersectionY, Color.green);
            }

            return new Vector3
            {
                x = Vector3.Distance(start, intersectionX) / 2.0F - minPlayerDistance.x,
                z = Vector3.Distance(start, intersectionY) / 2.0F - minPlayerDistance.y
            };
        }

        private Vector3 CalcWorldOffset()
        {
            Vector3 maxOffset = CalcMaxOffset();

            // Calc rotated axes
            Quaternion rotation = Quaternion.AngleAxis(cameraRotation, Vector3.up);
            Vector3 xOffsetAxis = rotation * new Vector3(1.0F, 0.0F, 0.0F);
            Vector3 zOffsetAxis = rotation * new Vector3(0.0F, 0.0F, 1.0F);

            Vector3 offset = Vector3.Scale(maxOffset, new Vector3(screenPos.x, 0.0F, screenPos.y));
            Vector3 worldOffset = xOffsetAxis * offset.x + zOffsetAxis * offset.z;

            if (Log.IsDebugEnabled)
            {
                // Axes
                Debug.DrawLine(Vector3.zero, xOffsetAxis, Color.red);
                Debug.DrawLine(Vector3.zero, zOffsetAxis, Color.blue);

                Debug.DrawLine(Vector3.zero, offset, Color.magenta);
                Debug.DrawLine(Vector3.zero, worldOffset, Color.cyan);
            }

            return worldOffset;
        }

        private void AdjustForViewZone(ref Vector3 worldOffset)
        {
            if (!viewZone)
            {
                return;
            }
            ViewZoneAdjuster adjuster = new ViewZoneAdjuster(this);
            adjuster.Adjust(ref worldOffset);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !Log.IsDebugEnabled)
            {
                return;
            }
            Gizmos.color = Color.cyan;
            Vector3 cursorPosViewport =
                controlled.ScreenToViewportPoint(NormalizedScreenToPixels(cursorPos));
            cursorPosViewport.z = 2.0F;
            Gizmos.DrawCube(controlled.ViewportToWorldPoint(cursorPosViewport), Vector3.one);
            Gizmos.color = Color.red;
            Vector3 offsetViewport =
                controlled.ScreenToViewportPoint(NormalizedScreenToPixels(screenPos));
            offsetViewport.z = 2.0F;
            Gizmos.DrawCube(controlled.ViewportToWorldPoint(offsetViewport), Vector3.one);
        }

        private void OnDisable()
        {
            gameInput.Player.CursorPosition.performed -= OnCursor;
        }

        private void OnDestroy()
        {
            gameInput.Player.CursorPosition.performed -= OnCursor;
        }

        private void OnCursor(InputAction.CallbackContext context)
        {
            cursorPos = NormalizeScreenPoint(context.ReadValue<Vector2>());
        }

        private Vector2 NormalizeScreenPoint(Vector2 point)
        {
            return new Vector2
            {
                x = Mathf.Lerp(-1.0F, 1.0F, Mathf.Clamp01(point.x / controlled.pixelWidth)),
                y = Mathf.Lerp(-1.0F, 1.0F, Mathf.Clamp01(point.y / controlled.pixelHeight))
            };
        }

        private Vector2 NormalizedScreenToPixels(Vector2 point)
        {
            return new Vector2
            {
                x = (point.x + 1.0F) / 2.0F * controlled.pixelWidth,
                y = (point.y + 1.0F) / 2.0F * controlled.pixelHeight
            };
        }

        class ViewZoneAdjuster
        {
            private readonly CameraController controller;
            private readonly LevelZone viewZone;

            private readonly Vector3 oldOffset;
            private readonly Vector3 lowerLeft;
            private readonly Vector3 upperLeft;
            private readonly Vector3 upperRight;
            private readonly Vector3 lowerRight;

            private readonly Vector3[] rect = new Vector3[4];

            public ViewZoneAdjuster(CameraController controller)
            {
                this.controller = controller;
                viewZone = controller.viewZone;

                oldOffset = controller.offsetObject.transform.localPosition;
                lowerLeft = GetViewZoneGroundPoint(new Vector2(-1.0F, -1.0F));
                upperLeft = GetViewZoneGroundPoint(new Vector2(-1.0F, 1.0F));
                upperRight = GetViewZoneGroundPoint(new Vector2(1.0F, 1.0F));
                lowerRight = GetViewZoneGroundPoint(new Vector2(1.0F, -1.0F));
            }

            private Vector3 GetViewZoneGroundPoint(Vector2 screenPoint)
            {
                screenPoint = controller.NormalizedScreenToPixels(screenPoint);
                Ray ray = controller.controlled.ScreenPointToRay(screenPoint);
                return GeometryUtils.GroundAndRayIntersection(ray).GetValueOrDefault() - oldOffset;
            }

            private void SetRect(Vector3 offset)
            {
                rect[0] = lowerLeft + offset;
                rect[1] = upperLeft + offset;
                rect[2] = upperRight + offset;
                rect[3] = lowerRight + offset;

                // if (Log.IsDebugEnabled)
                // {
                //     Debug.DrawLine(rect[0], rect[1], Color.yellow);
                //     Debug.DrawLine(rect[1], rect[2], Color.yellow);
                //     Debug.DrawLine(rect[2], rect[3], Color.yellow);
                //     Debug.DrawLine(rect[3], rect[0], Color.yellow);
                // }
            }

            public void Adjust(ref Vector3 worldOffset)
            {
                if (Log.IsDebugEnabled)
                {
                    Debug.DrawLine(lowerLeft, upperLeft, Color.white);
                    Debug.DrawLine(upperLeft, upperRight, Color.white);
                    Debug.DrawLine(upperRight, lowerRight, Color.white);
                    Debug.DrawLine(lowerRight, lowerLeft, Color.white);
                }

                SetRect(worldOffset);

                // Do binary search for view zone border
                // If mag is less than negative initial mag there's no point to continue
                // TODO: Re-implement this with proper math. BS with O(log n) still takes some time.
                if (!viewZone.IsWorldPolygonInZone(rect))
                {
                    BinarySearch(ref worldOffset);
                }

                if (Log.IsDebugEnabled)
                {
                    Debug.DrawLine(rect[0], rect[1], Color.magenta);
                    Debug.DrawLine(rect[1], rect[2], Color.magenta);
                    Debug.DrawLine(rect[2], rect[3], Color.magenta);
                    Debug.DrawLine(rect[3], rect[0], Color.magenta);
                }
            }

            private void BinarySearch(ref Vector3 worldOffset)
            {
                Vector3 normalizedWorldOffset = worldOffset.normalized;
                float initialMag = worldOffset.magnitude;
                float mag = initialMag;

                float step = ViewZoneAdjustStep;
                bool movingIn = true;
                bool wasInside = false;

                int iterations = 0;
                do
                {
                    // Update rect
                    worldOffset = normalizedWorldOffset * mag;
                    SetRect(worldOffset);

                    bool inside = viewZone.IsWorldPolygonInZone(rect);
                    // If rect inside and moving in or rect outside and moving out, flip moving dir
                    if (movingIn == inside)
                    {
                        movingIn = !movingIn;
                    }

                    mag += movingIn ? -step : step;

                    // If crossed the border, divide step by 2
                    if (wasInside != inside)
                    {
                        step /= 2.0F;
                    }

                    wasInside = inside;
                    iterations++;
                }
                while (step > ViewZoneAdjustError && mag > -initialMag && iterations < 100);

                Log.Debug()?.Call($"Out of view zone: " +
                                  $"shifting {initialMag - mag} in {iterations} iterations");
            }
        }
    }
}
