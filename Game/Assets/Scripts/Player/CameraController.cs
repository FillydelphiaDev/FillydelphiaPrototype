using System;
using log4net;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CameraController));

        [Tooltip("Offset object should not be moved.")]
        [SerializeField]
        private GameObject offsetObject = null;

        [SerializeField]
        private GameObject player = null;

        [Header("Settings")]
        [Tooltip("Minimal size of camera's viewport. Zoom will be adjusted to fit this size.")]
        [SerializeField]
        private float minViewSize = 20.0F;

        [Tooltip("The max camera offset by X and Z axes.")]
        [SerializeField]
        private Vector2 maxViewOffset = new Vector2(10.0F, 10.0F);

        [Tooltip("Speed of camera adjusting for mouse pos. Time is current distance " +
                 "and value is max adjusted distance per frame.")]
        [SerializeField]
        private AnimationCurve adjustingSpeed = AnimationCurve.EaseInOut(0.0F, 0.02F, 1.0F, 0.1F);

        private Camera controlled;

        private GameInput gameInput;

        // Normalized -1 to -1
        private Vector2 cursorPos;
        private Vector2 screenPos;

        private void Awake()
        {
            if (!offsetObject)
            {
                throw new InvalidOperationException("Pivot object should be set");
            }
            if (!player)
            {
                throw new InvalidOperationException("Player object should be set");
            }
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
            ApplyWorldOffset();
        }

        private void AdjustCameraZoom()
        {
            float aspect = controlled.pixelWidth / (float) controlled.pixelHeight;
            // Make sure that min view size is consistent for all screens
            if (aspect < 1.0F)
            {
                // If horizontal dim is the small one - adjust for aspect ratio
                controlled.orthographicSize = minViewSize / aspect / 2.0F;
            }
            else
            {
                controlled.orthographicSize = minViewSize / 2.0F;
            }
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

        private void ApplyWorldOffset()
        {
            Vector3 offset = Vector3.Scale(new Vector3(maxViewOffset.x, 1.0F, maxViewOffset.y),
                new Vector3(screenPos.x, 0.0F, screenPos.y));
            Vector3 worldOffset = GeometryUtils.GameImpliedRight * offset.x +
                                  GeometryUtils.GameImpliedForward * offset.z;

            if (Log.IsDebugEnabled)
            {
                Vector3 playerPos = player.transform.position;
                Debug.DrawRay(playerPos, offset, Color.magenta);
                Debug.DrawRay(playerPos, worldOffset, Color.cyan);
            }

            offsetObject.transform.localPosition = worldOffset;
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
            gameInput.Player.CursorPosition.Disable();
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
    }
}
