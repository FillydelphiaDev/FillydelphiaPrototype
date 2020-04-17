using System;
using log4net;
using UnityEngine;
using Utils;

namespace Character
{
    [RequireComponent(typeof(Character), typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CharacterMovement));

        private const float MaxPossibleSpeed = 100.0F;
        private const float RotationError = 0.001F;

        private ILog charLog;

        [SerializeField]
        private float walkingSpeed = 7.0F;

        public float WalkingSpeed
        {
            get => walkingSpeed;
            set
            {
                CheckSpeedArg(value);
                walkingSpeed = value;
                charLog.Debug()?.Call($"Walking speed set to {value}");
            }
        }

        [SerializeField]
        private float runningSpeed = 12.0F;

        public float RunningSpeed
        {
            get => runningSpeed;
            set
            {
                CheckSpeedArg(value);
                runningSpeed = value;
                charLog.Debug()?.Call($"Running speed set to {value}");
            }
        }

        [SerializeField]
        private float sneakingSpeed = 4.0F;

        public float SneakingSpeed
        {
            get => sneakingSpeed;
            set
            {
                CheckSpeedArg(value);
                sneakingSpeed = value;
                charLog.Debug()?.Call($"Sneaking speed set to {value}");
            }
        }

        [Tooltip("How many degrees can character rotate in one second?")]
        [SerializeField]
        private float rotationSpeed = 720.0F;

        public float RotationSpeed
        {
            get => rotationSpeed;
            set
            {
                if (rotationSpeed < 0.0F)
                {
                    throw new ArgumentException("Rotation speed can't be less than 0");
                }
                rotationSpeed = value;
                charLog.Debug()?.Call($"Rotation speed set to {value}");
            }
        }

        [Tooltip("Update on physics update or on frame update? The latter is for player chars.")]
        [SerializeField]
        private bool updateEachFrame = false;

        // Is this worth it?
        private new Transform transform;

        private Character character;
        private CharacterController controller;

        private MovementMode currentMode;

        public MovementMode Mode
        {
            get => currentMode;
            set
            {
                currentMode = value;
                charLog.Debug()?.Call($"Movement mode set to {value}");
            }
        }

        private Vector2 direction;

        /// <summary>
        /// This is ground vector.
        /// This is not always the current moving direction as character has rotation speed.
        /// </summary>
        public Vector2 Direction
        {
            get => direction;
            set
            {
                if (Mathf.Approximately(value.sqrMagnitude, 0.0F))
                {
                    throw new ArgumentException("Direction can't be zero-length vector");
                }
                direction = value.normalized;
                charLog.Debug()?.Call($"Direction set to {value.ToStringG3()}");
            }
        }

        private bool moving;

        public bool Moving
        {
            get => moving;
            set
            {
                moving = value;
                charLog.Debug()?.Call($"Moving state set to {value}");
            }
        }

        private void OnValidate()
        {
            walkingSpeed = Mathf.Clamp(walkingSpeed, 0.0F, MaxPossibleSpeed);
            runningSpeed = Mathf.Clamp(runningSpeed, 0.0F, MaxPossibleSpeed);
            sneakingSpeed = Mathf.Clamp(sneakingSpeed, 0.0F, MaxPossibleSpeed);
            rotationSpeed = Mathf.Clamp(rotationSpeed, 0.0F, float.MaxValue);
        }

        private void Awake()
        {
            charLog = CommonUtils.GetInstanceLogger<CharacterMovement>(GetInstanceID());
            transform = base.transform;
            character = GetComponent<Character>();
            controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            if (!updateEachFrame)
            {
                UpdateMovement(Time.fixedDeltaTime);
            }
        }

        private void Update()
        {
            if (updateEachFrame)
            {
                UpdateMovement(Time.deltaTime);
            }
        }

        private void UpdateMovement(float time)
        {
            Rotate(time);
            Move(time);
        }

        private void Rotate(float time)
        {
            Quaternion rotation = transform.rotation;
            Vector2 currentDir = (rotation * Vector3.forward).FromWorldToGround();

            // Skip rotation if angle is unnoticeable
            float angle = Vector2.Angle(currentDir, direction);
            if (angle < RotationError)
            {
                return;
            }

            // Raise an event
            RotationEvent rotationEvent =
                new RotationEvent(currentDir, direction, true, rotationSpeed);
            character.Events.Dispatch(ref rotationEvent);
            if (!rotationEvent.CanRotate)
            {
                charLog.Debug()?.Call($"Rotation was cancelled by event");
                return;
            }

            // Clamp rotation per time
            float modifiedRotationSpeed =
                Mathf.Clamp(rotationEvent.RotationSpeed, 0.0F, float.MaxValue);
            float delta = Mathf.Min(modifiedRotationSpeed * time, angle);

            // Apply delta rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction.FromGroundToWorld());
            rotation = Quaternion.RotateTowards(rotation, targetRotation, delta);
            transform.rotation = rotation;

            if (Log.IsDebugEnabled)
            {
                Vector3 player = transform.position;
                // Current dir
                Debug.DrawRay(player, currentDir.FromGroundToWorld(), Color.cyan);
                // Target dir
                Debug.DrawRay(player, targetRotation * Vector3.forward, Color.blue);
                // New dir
                Debug.DrawRay(player, rotation * Vector3.forward,
                    Color.magenta);
            }
            if (charLog.IsDebugEnabled)
            {
                Vector2 newDir = (rotation * Vector3.forward).FromWorldToGround();
                charLog.Debug($"Rotated for {angle} deg, new dir {newDir.ToStringG3()}");
            }
        }

        private void Move(float time)
        {
            if (!moving)
            {
                return;
            }

            Vector2 dir = (transform.rotation * Vector3.forward).FromWorldToGround();

            // Raise mode event
            MovementModeEvent modeEvent = new MovementModeEvent(dir, currentMode);
            character.Events.Dispatch(ref modeEvent);
            if (charLog.IsDebugEnabled && currentMode != modeEvent.Mode)
            {
                charLog.Debug($"Mode was changed by event from {currentMode} to {modeEvent.Mode}");
            }

            float speed = GetSpeedForMode(modeEvent.Mode);
            
            // Raise movement event
            MovementEvent movementEvent = new MovementEvent(dir, currentMode, time, true, speed);
            character.Events.Dispatch(ref movementEvent);

            if (!movementEvent.CanMove)
            {
                charLog.Debug()?.Call($"Movement was cancelled by event");
                return;
            }

            speed = Mathf.Clamp(movementEvent.Speed, 0.0F, MaxPossibleSpeed);
            Vector3 delta = dir.FromGroundToWorld() * (speed * time);

            CollisionFlags flags = controller.Move(delta);
            if (flags == CollisionFlags.None)
            {
                charLog?.Debug()?.Call($"{modeEvent.Mode} {delta.magnitude} in dir " +
                                       $"{direction.ToStringG3()} at speed {speed}");
            }
            else
            {
                charLog?.Debug()?.Call(
                    $"{modeEvent.Mode} {delta.magnitude} in dir {direction.ToStringG3()}" +
                    $"at speed {speed} and colliding {flags}");
            }
        }

        private static void CheckSpeedArg(float speed)
        {
            if (speed < 0.0F || speed > MaxPossibleSpeed)
            {
                throw new ArgumentException("Speed can't be lower than 0 or bigger than " +
                                            MaxPossibleSpeed);
            }
        }

        private float GetSpeedForMode(MovementMode mode)
        {
            switch (mode)
            {
                case MovementMode.Walking:
                    return walkingSpeed;
                case MovementMode.Running:
                    return runningSpeed;
                case MovementMode.Sneaking:
                    return sneakingSpeed;
            }
            throw new InvalidOperationException();
        }

        public enum MovementMode
        {
            Walking,
            Running,
            Sneaking
        }

        public struct RotationEvent
        {
            // Info
            public Vector2 CurrentDirection { get; }
            public Vector2 TargetDirection { get; }

            public bool CanRotate { get; set; }
            public float RotationSpeed { get; set; }

            public RotationEvent(Vector2 currentDirection, Vector2 targetDirection, bool canRotate,
                float rotationSpeed)
            {
                CurrentDirection = currentDirection;
                TargetDirection = targetDirection;
                CanRotate = canRotate;
                RotationSpeed = rotationSpeed;
            }
        }

        public struct MovementModeEvent
        {
            // Info
            public Vector2 Direction { get; }

            public MovementMode Mode { get; set; }

            public MovementModeEvent(Vector2 direction, MovementMode mode)
            {
                Direction = direction;
                Mode = mode;
            }
        }

        public struct MovementEvent
        {
            // Info
            public Vector2 Direction { get; }
            public MovementMode Mode { get; }
            public float Time { get; }

            public bool CanMove { get; set; }
            public float Speed { get; set; }

            public MovementEvent(Vector2 direction, MovementMode mode, float time, bool canMove,
                float speed)
            {
                Direction = direction;
                Mode = mode;
                Time = time;
                CanMove = canMove;
                Speed = speed;
            }
        }
    }
}
