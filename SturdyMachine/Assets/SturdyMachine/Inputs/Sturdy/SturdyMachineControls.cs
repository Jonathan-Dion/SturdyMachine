// GENERATED AUTOMATICALLY FROM 'Assets/SturdyMachine/Inputs/Sturdy/SturdyMachineControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @SturdyMachineControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @SturdyMachineControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SturdyMachineControls"",
    ""maps"": [
        {
            ""name"": ""Deflection"",
            ""id"": ""5bceb97c-17b1-4a82-b7f6-09cc8d1b5777"",
            ""actions"": [
                {
                    ""name"": ""Neutral"",
                    ""type"": ""Button"",
                    ""id"": ""bb6bd55e-a20d-4579-a003-8261b9f4e282"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""d550b741-61cc-4f96-b03d-9b1c17c39d09"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""4e352f11-98be-4345-9f10-059956ac7759"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Evasion"",
                    ""type"": ""Button"",
                    ""id"": ""bce82b26-14ea-489f-a98d-7aab3cf5eb6c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bc205809-aba6-4f4f-8316-0ec6fe133e05"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Neutral"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee4d1f07-5ac0-4f23-aacd-f486b83cad66"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f58008ee-0b83-4814-82c2-1d6d6a95fce5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d1c069e-3166-4751-ad5e-c539d68b8179"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Evasion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Sweep"",
            ""id"": ""e6dad343-7ef0-40c9-b2f8-f86b389f2ef2"",
            ""actions"": [
                {
                    ""name"": ""Neutral"",
                    ""type"": ""Button"",
                    ""id"": ""708735c3-7a9d-48a4-bd18-dec217e58a69"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7f1eb41a-7542-4550-a5a1-536cb9df3e5a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Neutral"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Stance"",
            ""id"": ""1d749fc6-a26f-4152-97ff-dfa3779c4033"",
            ""actions"": [
                {
                    ""name"": ""Strikes"",
                    ""type"": ""Button"",
                    ""id"": ""a5496e6b-12d6-4b7d-9a6b-7679d8dc31ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""Heavy"",
                    ""type"": ""Button"",
                    ""id"": ""27810590-e2da-4b53-9de9-1d28f8734dc1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""DeathBlow"",
                    ""type"": ""Button"",
                    ""id"": ""8bdbbd94-72b1-440a-a94e-b784750fe7af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f7d56bd7-c703-4516-82a0-65c8e73d9518"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Strikes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""462c5450-f9e8-452f-ac23-ca25c6693de8"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38dcd145-3824-4a2e-982b-755a767bd181"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""DeathBlow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Focus"",
            ""id"": ""d0c6f427-ff69-4f1f-93fb-b41909aff72b"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""0d1279cb-41ff-4e9c-b365-4e5f7cfdd7dc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""5fc2feda-1e60-4573-8316-b20f068e4e57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f1f5c3b4-fd59-4254-9c3c-38b6b4e62b6c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f87f9ec-5e41-41c6-a5f7-93030a646149"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard/Mouse"",
            ""bindingGroup"": ""Keyboard/Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<VirtualMouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Deflection
        m_Deflection = asset.FindActionMap("Deflection", throwIfNotFound: true);
        m_Deflection_Neutral = m_Deflection.FindAction("Neutral", throwIfNotFound: true);
        m_Deflection_Right = m_Deflection.FindAction("Right", throwIfNotFound: true);
        m_Deflection_Left = m_Deflection.FindAction("Left", throwIfNotFound: true);
        m_Deflection_Evasion = m_Deflection.FindAction("Evasion", throwIfNotFound: true);
        // Sweep
        m_Sweep = asset.FindActionMap("Sweep", throwIfNotFound: true);
        m_Sweep_Neutral = m_Sweep.FindAction("Neutral", throwIfNotFound: true);
        // Stance
        m_Stance = asset.FindActionMap("Stance", throwIfNotFound: true);
        m_Stance_Strikes = m_Stance.FindAction("Strikes", throwIfNotFound: true);
        m_Stance_Heavy = m_Stance.FindAction("Heavy", throwIfNotFound: true);
        m_Stance_DeathBlow = m_Stance.FindAction("DeathBlow", throwIfNotFound: true);
        // Focus
        m_Focus = asset.FindActionMap("Focus", throwIfNotFound: true);
        m_Focus_Left = m_Focus.FindAction("Left", throwIfNotFound: true);
        m_Focus_Right = m_Focus.FindAction("Right", throwIfNotFound: true);
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

    // Deflection
    private readonly InputActionMap m_Deflection;
    private IDeflectionActions m_DeflectionActionsCallbackInterface;
    private readonly InputAction m_Deflection_Neutral;
    private readonly InputAction m_Deflection_Right;
    private readonly InputAction m_Deflection_Left;
    private readonly InputAction m_Deflection_Evasion;
    public struct DeflectionActions
    {
        private @SturdyMachineControls m_Wrapper;
        public DeflectionActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Neutral => m_Wrapper.m_Deflection_Neutral;
        public InputAction @Right => m_Wrapper.m_Deflection_Right;
        public InputAction @Left => m_Wrapper.m_Deflection_Left;
        public InputAction @Evasion => m_Wrapper.m_Deflection_Evasion;
        public InputActionMap Get() { return m_Wrapper.m_Deflection; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DeflectionActions set) { return set.Get(); }
        public void SetCallbacks(IDeflectionActions instance)
        {
            if (m_Wrapper.m_DeflectionActionsCallbackInterface != null)
            {
                @Neutral.started -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnNeutral;
                @Neutral.performed -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnNeutral;
                @Neutral.canceled -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnNeutral;
                @Right.started -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnRight;
                @Left.started -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnLeft;
                @Evasion.started -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnEvasion;
                @Evasion.performed -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnEvasion;
                @Evasion.canceled -= m_Wrapper.m_DeflectionActionsCallbackInterface.OnEvasion;
            }
            m_Wrapper.m_DeflectionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Neutral.started += instance.OnNeutral;
                @Neutral.performed += instance.OnNeutral;
                @Neutral.canceled += instance.OnNeutral;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Evasion.started += instance.OnEvasion;
                @Evasion.performed += instance.OnEvasion;
                @Evasion.canceled += instance.OnEvasion;
            }
        }
    }
    public DeflectionActions @Deflection => new DeflectionActions(this);

    // Sweep
    private readonly InputActionMap m_Sweep;
    private ISweepActions m_SweepActionsCallbackInterface;
    private readonly InputAction m_Sweep_Neutral;
    public struct SweepActions
    {
        private @SturdyMachineControls m_Wrapper;
        public SweepActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Neutral => m_Wrapper.m_Sweep_Neutral;
        public InputActionMap Get() { return m_Wrapper.m_Sweep; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SweepActions set) { return set.Get(); }
        public void SetCallbacks(ISweepActions instance)
        {
            if (m_Wrapper.m_SweepActionsCallbackInterface != null)
            {
                @Neutral.started -= m_Wrapper.m_SweepActionsCallbackInterface.OnNeutral;
                @Neutral.performed -= m_Wrapper.m_SweepActionsCallbackInterface.OnNeutral;
                @Neutral.canceled -= m_Wrapper.m_SweepActionsCallbackInterface.OnNeutral;
            }
            m_Wrapper.m_SweepActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Neutral.started += instance.OnNeutral;
                @Neutral.performed += instance.OnNeutral;
                @Neutral.canceled += instance.OnNeutral;
            }
        }
    }
    public SweepActions @Sweep => new SweepActions(this);

    // Stance
    private readonly InputActionMap m_Stance;
    private IStanceActions m_StanceActionsCallbackInterface;
    private readonly InputAction m_Stance_Strikes;
    private readonly InputAction m_Stance_Heavy;
    private readonly InputAction m_Stance_DeathBlow;
    public struct StanceActions
    {
        private @SturdyMachineControls m_Wrapper;
        public StanceActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Strikes => m_Wrapper.m_Stance_Strikes;
        public InputAction @Heavy => m_Wrapper.m_Stance_Heavy;
        public InputAction @DeathBlow => m_Wrapper.m_Stance_DeathBlow;
        public InputActionMap Get() { return m_Wrapper.m_Stance; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StanceActions set) { return set.Get(); }
        public void SetCallbacks(IStanceActions instance)
        {
            if (m_Wrapper.m_StanceActionsCallbackInterface != null)
            {
                @Strikes.started -= m_Wrapper.m_StanceActionsCallbackInterface.OnStrikes;
                @Strikes.performed -= m_Wrapper.m_StanceActionsCallbackInterface.OnStrikes;
                @Strikes.canceled -= m_Wrapper.m_StanceActionsCallbackInterface.OnStrikes;
                @Heavy.started -= m_Wrapper.m_StanceActionsCallbackInterface.OnHeavy;
                @Heavy.performed -= m_Wrapper.m_StanceActionsCallbackInterface.OnHeavy;
                @Heavy.canceled -= m_Wrapper.m_StanceActionsCallbackInterface.OnHeavy;
                @DeathBlow.started -= m_Wrapper.m_StanceActionsCallbackInterface.OnDeathBlow;
                @DeathBlow.performed -= m_Wrapper.m_StanceActionsCallbackInterface.OnDeathBlow;
                @DeathBlow.canceled -= m_Wrapper.m_StanceActionsCallbackInterface.OnDeathBlow;
            }
            m_Wrapper.m_StanceActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Strikes.started += instance.OnStrikes;
                @Strikes.performed += instance.OnStrikes;
                @Strikes.canceled += instance.OnStrikes;
                @Heavy.started += instance.OnHeavy;
                @Heavy.performed += instance.OnHeavy;
                @Heavy.canceled += instance.OnHeavy;
                @DeathBlow.started += instance.OnDeathBlow;
                @DeathBlow.performed += instance.OnDeathBlow;
                @DeathBlow.canceled += instance.OnDeathBlow;
            }
        }
    }
    public StanceActions @Stance => new StanceActions(this);

    // Focus
    private readonly InputActionMap m_Focus;
    private IFocusActions m_FocusActionsCallbackInterface;
    private readonly InputAction m_Focus_Left;
    private readonly InputAction m_Focus_Right;
    public struct FocusActions
    {
        private @SturdyMachineControls m_Wrapper;
        public FocusActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_Focus_Left;
        public InputAction @Right => m_Wrapper.m_Focus_Right;
        public InputActionMap Get() { return m_Wrapper.m_Focus; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FocusActions set) { return set.Get(); }
        public void SetCallbacks(IFocusActions instance)
        {
            if (m_Wrapper.m_FocusActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_FocusActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_FocusActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_FocusActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_FocusActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_FocusActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_FocusActionsCallbackInterface.OnRight;
            }
            m_Wrapper.m_FocusActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
            }
        }
    }
    public FocusActions @Focus => new FocusActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard/Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IDeflectionActions
    {
        void OnNeutral(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnEvasion(InputAction.CallbackContext context);
    }
    public interface ISweepActions
    {
        void OnNeutral(InputAction.CallbackContext context);
    }
    public interface IStanceActions
    {
        void OnStrikes(InputAction.CallbackContext context);
        void OnHeavy(InputAction.CallbackContext context);
        void OnDeathBlow(InputAction.CallbackContext context);
    }
    public interface IFocusActions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
    }
}
