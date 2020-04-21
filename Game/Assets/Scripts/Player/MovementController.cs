using Character;
using log4net;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    [RequireComponent(typeof(CharacterMovement))]
    public class MovementController : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MovementController));

        [SerializeField]
        private float walkingSpeed = 7.0F;

        [SerializeField]
        private float sprintingSpeed = 12.0F;

        [SerializeField]
        private float sneakingSpeed = 2.0F;

        [SerializeField]
        private float fastSneakingSpeed = 4.0F;

        private CharacterMovement characterMovement;

        private GameInput gameInput;

        private bool moving;
        private Vector3 direction;
        private bool sprinting;
        private bool sneaking;

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
            gameInput = new GameInput();
            gameInput.Player.Movement.started += OnMovementStarted;
            gameInput.Player.Movement.performed += OnMovementPerformed;
            gameInput.Player.Movement.canceled += OnMovementCanceled;
            gameInput.Player.Sprint.started += OnSprintStarted;
            gameInput.Player.Sprint.canceled += OnSprintCanceled;
            gameInput.Player.Sneak.started += OnSneakStarted;
            gameInput.Player.Sneak.canceled += OnSneakCanceled;
        }

        private void OnEnable()
        {
            gameInput.Player.Movement.Enable();
            gameInput.Player.Sprint.Enable();
            gameInput.Player.Sneak.Enable();
        }

        private void OnMovementStarted(InputAction.CallbackContext context)
        {
            moving = true;
            Log.Debug()?.Call("Movement started");
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            Vector2 raw = context.ReadValue<Vector2>();
            direction = GeometryUtils.CameraYRotation * new Vector3(raw.x, 0.0F, raw.y);
            Log.Debug()?.Call($"Movement performed: raw = {raw}, rotated = {direction}");
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            moving = false;
            Log.Debug()?.Call("Movement ended");
        }

        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            sprinting = true;
            Log.Debug()?.Call("Sprint started");
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            sprinting = false;
            Log.Debug()?.Call("Sprint ended");
        }

        private void OnSneakStarted(InputAction.CallbackContext context)
        {
            sneaking = true;
            Log.Debug()?.Call("Sneak started");
        }

        private void OnSneakCanceled(InputAction.CallbackContext context)
        {
            sneaking = false;
            Log.Debug()?.Call("Sneak ended");
        }

        private void Update()
        {
            characterMovement.Moving = moving;
            if (moving)
            {
                characterMovement.Direction = direction;
                if (sprinting && sneaking)
                {
                    characterMovement.Speed = fastSneakingSpeed;
                }
                else if (sprinting)
                {
                    characterMovement.Speed = sprintingSpeed;
                }
                else if (sneaking)
                {
                    characterMovement.Speed = sneakingSpeed;
                }
                else
                {
                    characterMovement.Speed = walkingSpeed;
                }
            }
            else
            {
                characterMovement.Speed = 0.0F;
            }
        }

        private void OnDisable()
        {
            gameInput.Player.Movement.Disable();
            gameInput.Player.Sprint.Disable();
            gameInput.Player.Sneak.Disable();
        }

        private void OnDestroy()
        {
            gameInput.Player.Movement.started -= OnMovementStarted;
            gameInput.Player.Movement.performed -= OnMovementPerformed;
            gameInput.Player.Movement.canceled -= OnMovementCanceled;
            gameInput.Player.Sprint.started -= OnSprintStarted;
            gameInput.Player.Sprint.canceled -= OnSprintCanceled;
            gameInput.Player.Sneak.started -= OnSneakStarted;
            gameInput.Player.Sneak.canceled -= OnSneakCanceled;
        }
    }
}
