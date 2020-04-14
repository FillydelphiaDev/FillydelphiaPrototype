// GENERATED AUTOMATICALLY FROM 'Assets/GameInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""e2c0eede-5a78-44c5-9235-6eb0b30c7fc1"",
            ""actions"": [
                {
                    ""name"": ""CursorDelta"",
                    ""type"": ""Button"",
                    ""id"": ""e42ca749-f29a-4445-be42-db06cb9bd1e2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CursorPosition"",
                    ""type"": ""Button"",
                    ""id"": ""8706afd4-edd9-4692-9c26-add4d3be8edd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d6b649d3-33c4-4b77-9cc8-6634367bb354"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""44f4f939-1dce-4215-b75c-a5dcaebb24ce"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_CursorDelta = m_Player.FindAction("CursorDelta", throwIfNotFound: true);
        m_Player_CursorPosition = m_Player.FindAction("CursorPosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_CursorDelta;
    private readonly InputAction m_Player_CursorPosition;
    public struct PlayerActions
    {
        private @GameInput m_Wrapper;
        public PlayerActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @CursorDelta => m_Wrapper.m_Player_CursorDelta;
        public InputAction @CursorPosition => m_Wrapper.m_Player_CursorPosition;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @CursorDelta.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDelta;
                @CursorDelta.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDelta;
                @CursorDelta.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDelta;
                @CursorPosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorPosition;
                @CursorPosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorPosition;
                @CursorPosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorPosition;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CursorDelta.started += instance.OnCursorDelta;
                @CursorDelta.performed += instance.OnCursorDelta;
                @CursorDelta.canceled += instance.OnCursorDelta;
                @CursorPosition.started += instance.OnCursorPosition;
                @CursorPosition.performed += instance.OnCursorPosition;
                @CursorPosition.canceled += instance.OnCursorPosition;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnCursorDelta(InputAction.CallbackContext context);
        void OnCursorPosition(InputAction.CallbackContext context);
    }
}
