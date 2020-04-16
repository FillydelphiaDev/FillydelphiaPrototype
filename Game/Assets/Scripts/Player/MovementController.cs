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

        private CharacterMovement characterMovement;

        private GameInput gameInput;

        private bool sprintActive;
        private bool sneakActive;

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
            characterMovement.Moving = true;
            Log.Debug()?.Call("Movement started");
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            Vector2 raw = context.ReadValue<Vector2>();
            Vector2 direction = characterMovement.Direction = raw.ApplyCameraRotation();
            Log.Debug()?.Call($"Movement performed: raw = {raw}, rotated = {direction}");
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            characterMovement.Moving = false;
            Log.Debug()?.Call("Movement ended");
        }

        private void OnSprintStarted(InputAction.CallbackContext context)
        {
            sprintActive = true;
            if (!sneakActive)
            {
                characterMovement.Mode = CharacterMovement.MovementMode.Running;
                Log.Debug()?.Call("Sprint started, Running mode set");
            }
            else
            {
                Log.Debug()?.Call("Sprint started, Running mode not set because Sneak is active");
            }
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            sprintActive = false;
            if (!sneakActive)
            {
                characterMovement.Mode = CharacterMovement.MovementMode.Walking;
                Log.Debug()?.Call("Sprint ended, Walking mode set");
            }
            else
            {
                Log.Debug()?.Call("Sprint ended, Walking mode not set because Sneak is active");
            }
        }

        private void OnSneakStarted(InputAction.CallbackContext context)
        {
            sneakActive = true;
            characterMovement.Mode = CharacterMovement.MovementMode.Sneaking;
            Log.Debug()?.Call("Sneak started, Sneaking mode set");
        }

        private void OnSneakCanceled(InputAction.CallbackContext context)
        {
            sneakActive = false;
            if (!sprintActive)
            {
                characterMovement.Mode = CharacterMovement.MovementMode.Walking;
                Log.Debug()?.Call("Sneak started, Walking mode set");
            }
            else
            {
                characterMovement.Mode = CharacterMovement.MovementMode.Running;
                Log.Debug()?.Call("Sneak started, Running mode set because Sprint is active");
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
