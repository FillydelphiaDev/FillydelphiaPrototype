using System;
using Character.Trait;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Character))]
    public class CharacterMovement : MonoBehaviour
    {
        private const float MaxPossibleSpeed = 100.0F;

        private const float RotationError = 0.001F;

        [SerializeField]
        private float walkingSpeed = 3.0F;

        public float WalkingSpeed
        {
            get => walkingSpeed;
            set
            {
                CheckSpeedArg(value);
                walkingSpeed = value;
            }
        }

        [SerializeField]
        private float runningSpeed = 7.0F;

        public float RunningSpeed
        {
            get => runningSpeed;
            set
            {
                CheckSpeedArg(value);
                runningSpeed = value;
            }
        }

        [SerializeField]
        private float sneakingSpeed = 2.0F;

        public float SneakingSpeed
        {
            get => sneakingSpeed;
            set
            {
                CheckSpeedArg(value);
                sneakingSpeed = value;
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
            }
        }

        [Tooltip("Update on physics update or on frame update? The latter is for player chars.")]
        [SerializeField]
        private bool updateEachFrame = false;

        private Character character;

        private MovementMode currentMode;

        public MovementMode Mode
        {
            get => currentMode;
            set => currentMode = value;
        }

        private Vector3 currentDirection;
        private Vector3 targetDirection;

        public Vector3 CurrentDirection
        {
            get => currentDirection;
            set
            {
                currentDirection = new Vector3(value.x, 0.0F, value.z).normalized;
                targetDirection = currentDirection;
            }
        }

        /// <summary>
        /// This is not always the current moving direction as character has rotation speed.
        /// </summary>
        private Vector3 Direction
        {
            get => targetDirection;
            set => targetDirection = new Vector3(value.x, 0.0F, value.z).normalized;
        }

        private bool moving;

        public bool Moving
        {
            get => moving;
            set => moving = value;
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
            character = GetComponent<Character>();
        }

        private void FixedUpdate()
        {
            if (!updateEachFrame)
            {
                DoWork(Time.fixedTime);
            }
        }

        private void Update()
        {
            if (updateEachFrame)
            {
                DoWork(Time.deltaTime);
            }
        }

        private void DoWork(float time)
        {
            Rotate(time);
            Move(time);
        }

        private void Rotate(float time)
        {
            Vector3 diff = targetDirection - currentDirection;
            if (Math.Abs(diff.x) < RotationError && Math.Abs(diff.z) < RotationError)
            {
                return;
            }
            if (!character.Modifiers.ApplyModifier<CanRotateTrait, bool>(true))
            {
                return;
            }

            float modifiedRotationSpeed =
                character.Modifiers.ApplyModifier<RotationSpeedTrait, float>(rotationSpeed);

            float maxAngle = modifiedRotationSpeed * time;
            float currentAngle = Vector3.Angle(currentDirection, targetDirection);
            float angle = Mathf.Min(currentAngle, maxAngle);

            currentDirection = Quaternion.Euler(0.0F, 0.0F, angle) * currentDirection;
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
            float speed = walkingSpeed;
            speed = character.Modifiers.ApplyModifier<MovementSpeedTrait, float>(speed);
            speed = character.Modifiers.ApplyModifier<WalkingSpeedTrait, float>(speed);
            speed = Mathf.Clamp(speed, 0.0F, MaxPossibleSpeed);
            transform.position += currentDirection * (speed * time);
        }

        private void Run(float time)
        {
            if (!character.Modifiers.ApplyModifier<CanRunTrait, bool>(true))
            {
                Walk(time);
            }
            float speed = runningSpeed;
            speed = character.Modifiers.ApplyModifier<MovementSpeedTrait, float>(speed);
            speed = character.Modifiers.ApplyModifier<RunningSpeedTrait, float>(speed);
            speed = Mathf.Clamp(speed, 0.0F, MaxPossibleSpeed);
            transform.position += currentDirection * (speed * time);
        }

        private void Sneak(float time)
        {
            if (!character.Modifiers.ApplyModifier<CanSneakTrait, bool>(true))
            {
                Walk(time);
            }
            float speed = sneakingSpeed;
            speed = character.Modifiers.ApplyModifier<MovementSpeedTrait, float>(speed);
            speed = character.Modifiers.ApplyModifier<SneakingSpeedTrait, float>(speed);
            speed = Mathf.Clamp(speed, 0.0F, MaxPossibleSpeed);
            transform.position += currentDirection * (speed * time);
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
