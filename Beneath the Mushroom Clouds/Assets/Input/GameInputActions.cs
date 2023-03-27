//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Input/GameInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f387f875-b58c-4d17-8b3b-5681021c3c38"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""365c6824-535e-445f-a231-8f41c2ddc589"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aiming"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a1221fb9-cd95-407e-87b3-0ac037c22f1a"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""8fd3c946-3f04-414c-a859-b576d049a8dd"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""0e4342d7-44f8-45fd-af0b-1fff3443ed1d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""9c84a3ee-a306-425c-8bbc-07a810888a7d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""be168291-a2ce-4ebc-ac6e-31e4f72646e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenInventory"",
                    ""type"": ""Button"",
                    ""id"": ""2c341c7a-f484-459a-86af-d890cb7f1bed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""aeed3f54-04ce-49a3-9c69-66928880c818"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PrimaryWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""86e147b3-8598-4555-94a3-83855b6c0c83"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SecondaryWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""8dd18acf-1956-4c96-8fea-9895067f0694"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CycleWeapon"",
                    ""type"": ""Value"",
                    ""id"": ""3e3e88e6-c238-4f9c-a0a7-55b07e48faae"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SwitchFiremode"",
                    ""type"": ""Button"",
                    ""id"": ""1a92d041-ecff-4d14-a2bf-9f5bbdbbd6d9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WSAD"",
                    ""id"": ""304c81a4-a460-45ac-bebd-ed7e5650ec2f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4bbc6ded-a15c-48e0-b600-bdd3dcff3c71"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""497b2784-bcba-4f2e-8396-1cf1cde4db14"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1a52cdf0-e0d3-40f1-877e-1e67dc8e9fd5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""904dbf7d-43c4-4707-b00c-75fda7bd5b23"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4cab7776-c93d-4c0c-99e2-126af58b9790"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aiming"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec140257-28c2-4d02-bd31-2a8836aeccbb"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79a7ab2b-8617-452b-a152-6d2b90642408"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3812b53e-659b-4545-b55c-3725a1a3b981"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68c3bf65-bfd0-45ab-bb95-9ab254c837ba"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30ad5bef-5520-4926-b8ae-670012dad816"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""815d99f8-7747-4dba-b136-e5d2cde7984f"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f805792-b126-4fee-b561-868fad223109"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8aa562f-110d-466e-9c99-8e73186dc6f4"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40f7e14a-dab7-451b-a93c-4c785f175142"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CycleWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""318dba54-b815-4462-a9d3-0ac2efd1c895"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchFiremode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""3d36d05f-361a-4fef-9b96-094810e29e57"",
            ""actions"": [
                {
                    ""name"": ""CloseInventory"",
                    ""type"": ""Button"",
                    ""id"": ""f4cf761e-7064-49d9-8ae4-5cfe6d7b1f85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""28dbc86e-86fc-4d2c-a7c2-eae8643c288d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""DebugSpawnItem"",
                    ""type"": ""Button"",
                    ""id"": ""b918b319-94e3-4301-a551-50aeb78b4314"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateItem"",
                    ""type"": ""Button"",
                    ""id"": ""dacb42d0-de11-4c87-a8df-6f0045569c12"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SplitStack"",
                    ""type"": ""Button"",
                    ""id"": ""b5f89659-d70c-4d89-a552-70f4c9ac2582"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""c1abeb7e-9d48-42f0-8ae2-86aa0cd4f970"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""QuickTransfer"",
                    ""type"": ""Button"",
                    ""id"": ""b29620f9-53ba-40ae-8382-08a78d493269"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""QuickEquip"",
                    ""type"": ""Button"",
                    ""id"": ""2724323c-8d9d-414d-bf19-731e0f0db523"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8c581553-f04e-4627-a7e4-9c5f4ebb8e93"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""088a3171-3385-428e-978e-b8f500368dc5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1b6a322-d3e3-42d9-86a0-242710c12a1b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugSpawnItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c6c579d-ee0a-469f-9ef3-6cb11e71e74c"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""287fd15d-f982-4b5d-9845-3e24082a77e3"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SplitStack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14d8fef9-c0c7-40bf-bf2b-78dac361094c"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c2cce7c-fbbf-4101-8e32-92d15d0a9f39"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickTransfer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""af7bfc5c-442e-414c-bbbc-6006bfbb2143"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuickEquip"",
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
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Aiming = m_Player.FindAction("Aiming", throwIfNotFound: true);
        m_Player_MousePosition = m_Player.FindAction("MousePosition", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
        m_Player_OpenInventory = m_Player.FindAction("OpenInventory", throwIfNotFound: true);
        m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
        m_Player_PrimaryWeapon = m_Player.FindAction("PrimaryWeapon", throwIfNotFound: true);
        m_Player_SecondaryWeapon = m_Player.FindAction("SecondaryWeapon", throwIfNotFound: true);
        m_Player_CycleWeapon = m_Player.FindAction("CycleWeapon", throwIfNotFound: true);
        m_Player_SwitchFiremode = m_Player.FindAction("SwitchFiremode", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_CloseInventory = m_UI.FindAction("CloseInventory", throwIfNotFound: true);
        m_UI_LeftClick = m_UI.FindAction("LeftClick", throwIfNotFound: true);
        m_UI_DebugSpawnItem = m_UI.FindAction("DebugSpawnItem", throwIfNotFound: true);
        m_UI_RotateItem = m_UI.FindAction("RotateItem", throwIfNotFound: true);
        m_UI_SplitStack = m_UI.FindAction("SplitStack", throwIfNotFound: true);
        m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
        m_UI_QuickTransfer = m_UI.FindAction("QuickTransfer", throwIfNotFound: true);
        m_UI_QuickEquip = m_UI.FindAction("QuickEquip", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Aiming;
    private readonly InputAction m_Player_MousePosition;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_Fire;
    private readonly InputAction m_Player_OpenInventory;
    private readonly InputAction m_Player_Reload;
    private readonly InputAction m_Player_PrimaryWeapon;
    private readonly InputAction m_Player_SecondaryWeapon;
    private readonly InputAction m_Player_CycleWeapon;
    private readonly InputAction m_Player_SwitchFiremode;
    public struct PlayerActions
    {
        private @GameInputActions m_Wrapper;
        public PlayerActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Aiming => m_Wrapper.m_Player_Aiming;
        public InputAction @MousePosition => m_Wrapper.m_Player_MousePosition;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @Fire => m_Wrapper.m_Player_Fire;
        public InputAction @OpenInventory => m_Wrapper.m_Player_OpenInventory;
        public InputAction @Reload => m_Wrapper.m_Player_Reload;
        public InputAction @PrimaryWeapon => m_Wrapper.m_Player_PrimaryWeapon;
        public InputAction @SecondaryWeapon => m_Wrapper.m_Player_SecondaryWeapon;
        public InputAction @CycleWeapon => m_Wrapper.m_Player_CycleWeapon;
        public InputAction @SwitchFiremode => m_Wrapper.m_Player_SwitchFiremode;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Aiming.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAiming;
                @Aiming.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAiming;
                @Aiming.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAiming;
                @MousePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @Sprint.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @Crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                @OpenInventory.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenInventory;
                @OpenInventory.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenInventory;
                @OpenInventory.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenInventory;
                @Reload.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @PrimaryWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryWeapon;
                @PrimaryWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryWeapon;
                @PrimaryWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryWeapon;
                @SecondaryWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryWeapon;
                @SecondaryWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryWeapon;
                @SecondaryWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryWeapon;
                @CycleWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleWeapon;
                @CycleWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleWeapon;
                @CycleWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleWeapon;
                @SwitchFiremode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchFiremode;
                @SwitchFiremode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchFiremode;
                @SwitchFiremode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchFiremode;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Aiming.started += instance.OnAiming;
                @Aiming.performed += instance.OnAiming;
                @Aiming.canceled += instance.OnAiming;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @OpenInventory.started += instance.OnOpenInventory;
                @OpenInventory.performed += instance.OnOpenInventory;
                @OpenInventory.canceled += instance.OnOpenInventory;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @PrimaryWeapon.started += instance.OnPrimaryWeapon;
                @PrimaryWeapon.performed += instance.OnPrimaryWeapon;
                @PrimaryWeapon.canceled += instance.OnPrimaryWeapon;
                @SecondaryWeapon.started += instance.OnSecondaryWeapon;
                @SecondaryWeapon.performed += instance.OnSecondaryWeapon;
                @SecondaryWeapon.canceled += instance.OnSecondaryWeapon;
                @CycleWeapon.started += instance.OnCycleWeapon;
                @CycleWeapon.performed += instance.OnCycleWeapon;
                @CycleWeapon.canceled += instance.OnCycleWeapon;
                @SwitchFiremode.started += instance.OnSwitchFiremode;
                @SwitchFiremode.performed += instance.OnSwitchFiremode;
                @SwitchFiremode.canceled += instance.OnSwitchFiremode;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_CloseInventory;
    private readonly InputAction m_UI_LeftClick;
    private readonly InputAction m_UI_DebugSpawnItem;
    private readonly InputAction m_UI_RotateItem;
    private readonly InputAction m_UI_SplitStack;
    private readonly InputAction m_UI_RightClick;
    private readonly InputAction m_UI_QuickTransfer;
    private readonly InputAction m_UI_QuickEquip;
    public struct UIActions
    {
        private @GameInputActions m_Wrapper;
        public UIActions(@GameInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @CloseInventory => m_Wrapper.m_UI_CloseInventory;
        public InputAction @LeftClick => m_Wrapper.m_UI_LeftClick;
        public InputAction @DebugSpawnItem => m_Wrapper.m_UI_DebugSpawnItem;
        public InputAction @RotateItem => m_Wrapper.m_UI_RotateItem;
        public InputAction @SplitStack => m_Wrapper.m_UI_SplitStack;
        public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
        public InputAction @QuickTransfer => m_Wrapper.m_UI_QuickTransfer;
        public InputAction @QuickEquip => m_Wrapper.m_UI_QuickEquip;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @CloseInventory.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseInventory;
                @CloseInventory.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseInventory;
                @CloseInventory.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseInventory;
                @LeftClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnLeftClick;
                @LeftClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnLeftClick;
                @LeftClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnLeftClick;
                @DebugSpawnItem.started -= m_Wrapper.m_UIActionsCallbackInterface.OnDebugSpawnItem;
                @DebugSpawnItem.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnDebugSpawnItem;
                @DebugSpawnItem.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnDebugSpawnItem;
                @RotateItem.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRotateItem;
                @RotateItem.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRotateItem;
                @RotateItem.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRotateItem;
                @SplitStack.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSplitStack;
                @SplitStack.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSplitStack;
                @SplitStack.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSplitStack;
                @RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                @QuickTransfer.started -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickTransfer;
                @QuickTransfer.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickTransfer;
                @QuickTransfer.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickTransfer;
                @QuickEquip.started -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickEquip;
                @QuickEquip.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickEquip;
                @QuickEquip.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnQuickEquip;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CloseInventory.started += instance.OnCloseInventory;
                @CloseInventory.performed += instance.OnCloseInventory;
                @CloseInventory.canceled += instance.OnCloseInventory;
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @DebugSpawnItem.started += instance.OnDebugSpawnItem;
                @DebugSpawnItem.performed += instance.OnDebugSpawnItem;
                @DebugSpawnItem.canceled += instance.OnDebugSpawnItem;
                @RotateItem.started += instance.OnRotateItem;
                @RotateItem.performed += instance.OnRotateItem;
                @RotateItem.canceled += instance.OnRotateItem;
                @SplitStack.started += instance.OnSplitStack;
                @SplitStack.performed += instance.OnSplitStack;
                @SplitStack.canceled += instance.OnSplitStack;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @QuickTransfer.started += instance.OnQuickTransfer;
                @QuickTransfer.performed += instance.OnQuickTransfer;
                @QuickTransfer.canceled += instance.OnQuickTransfer;
                @QuickEquip.started += instance.OnQuickEquip;
                @QuickEquip.performed += instance.OnQuickEquip;
                @QuickEquip.canceled += instance.OnQuickEquip;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAiming(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnOpenInventory(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnPrimaryWeapon(InputAction.CallbackContext context);
        void OnSecondaryWeapon(InputAction.CallbackContext context);
        void OnCycleWeapon(InputAction.CallbackContext context);
        void OnSwitchFiremode(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnCloseInventory(InputAction.CallbackContext context);
        void OnLeftClick(InputAction.CallbackContext context);
        void OnDebugSpawnItem(InputAction.CallbackContext context);
        void OnRotateItem(InputAction.CallbackContext context);
        void OnSplitStack(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnQuickTransfer(InputAction.CallbackContext context);
        void OnQuickEquip(InputAction.CallbackContext context);
    }
}
