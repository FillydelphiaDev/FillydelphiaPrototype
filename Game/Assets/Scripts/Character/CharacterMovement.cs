using System;
using Character.Trait;
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

            if (!character.Modifiers.ApplyModifier<CanRotateTrait, bool>(true))
            {
                return;
            }

            // Clamp rotation per time
            float modifiedRotationSpeed =
                character.Modifiers.ApplyModifier<RotationSpeedTrait, float>(rotationSpeed);
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
            if (!character.Modifiers.ApplyModifier<CanMoveTrait, bool>(true))
            {
                return;
            }

            switch (currentMode)
            {
                case MovementMode.Walking:
                    Walk(time);
                    break;
                case MovementMode.Running:
                    Run(time);
                    break;
                case MovementMode.Sneaking:
                    Sneak(time);
                    break;
            }
        }

        private void Walk(float time)
        {
            float speed = character.Modifiers.ApplyModifier<WalkingSpeedTrait, float>(walkingSpeed);
            ApplySpeed(speed, time);
        }

        private void Run(float time)
        {
            if (!character.Modifiers.ApplyModifier<CanRunTrait, bool>(true))
            {
                Walk(time);
            }
            float speed = character.Modifiers.ApplyModifier<RunningSpeedTrait, float>(runningSpeed);
            ApplySpeed(speed, time);
        }

        private void Sneak(float time)
        {
            if (!character.Modifiers.ApplyModifier<CanSneakTrait, bool>(true))
            {
                Walk(time);
            }
            float speed =
                character.Modifiers.ApplyModifier<SneakingSpeedTrait, float>(sneakingSpeed);
            ApplySpeed(speed, time);
        }

        private void ApplySpeed(float speed, float time)
        {
            speed = character.Modifiers.ApplyModifier<MovementSpeedTrait, float>(speed);
            speed = Mathf.Clamp(speed, 0.0F, MaxPossibleSpeed);
            Vector3 delta = transform.rotation * Vector3.forward * (speed * time);

            CollisionFlags flags = controller.Move(delta);
            if (flags == CollisionFlags.None)
            {
                charLog?.Debug()?.Call($"{currentMode} {delta.magnitude} in dir " +
                                       $"{direction.ToStringG3()} at speed {speed}");
            }
            else
            {
                charLog?.Debug()?.Call(
                    $"{currentMode} {delta.magnitude} in dir {direction.ToStringG3()}" +
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

        public enum MovementMode
        {
            Walking,
            Running,
            Sneaking
        }

        /// <summary>
        /// This includes all movement modes. E.g. if this is false, then char won't move at all.
        /// </summary>
        public sealed class CanMoveTrait : Trait<bool>
        {
            private CanMoveTrait()
            {
            }
        }

        /// <summary>
        /// Use to modify any movement mode speed.
        /// </summary>
        public sealed class MovementSpeedTrait : Trait<float>
        {
            private MovementSpeedTrait()
            {
            }
        }

        public sealed class WalkingSpeedTrait : Trait<float>
        {
            private WalkingSpeedTrait()
            {
            }
        }

        public sealed class CanRunTrait : Trait<bool>
        {
            private CanRunTrait()
            {
            }
        }

        public sealed class RunningSpeedTrait : Trait<float>
        {
            private RunningSpeedTrait()
            {
            }
        }

        public sealed class CanSneakTrait : Trait<bool>
        {
            private CanSneakTrait()
            {
            }
        }

        public sealed class SneakingSpeedTrait : Trait<float>
        {
            private SneakingSpeedTrait()
            {
            }
        }

        public sealed class CanRotateTrait : Trait<bool>
        {
            private CanRotateTrait()
            {
            }
        }

        public sealed class RotationSpeedTrait : Trait<float>
        {
            private RotationSpeedTrait()
            {
            }
        }
    }
}
