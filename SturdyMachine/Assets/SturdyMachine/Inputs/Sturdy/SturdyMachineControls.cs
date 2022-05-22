// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/SturdyMachineControls.inputactions'

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
            ""name"": ""Strikes"",
            ""id"": ""2465fc91-7763-48b3-84b0-3c367c0bc351"",
            ""actions"": [
                {
                    ""name"": ""Strikes"",
                    ""type"": ""Button"",
                    ""id"": ""8dd3bc04-8d6a-428d-ac60-4f252997f3f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d748d7bb-71a6-4651-ad4d-b7c3cc1cb4b2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Strikes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Heavy"",
            ""id"": ""c43d169a-acfd-42b0-b145-7c746d8a1d9d"",
            ""actions"": [
                {
                    ""name"": ""Heavy"",
                    ""type"": ""Button"",
                    ""id"": ""fa35949f-0239-4bd9-88d4-dbad9d69abfb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6113a42b-39ca-4090-a8ee-c5bb089041ce"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Heavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""DeathBlow"",
            ""id"": ""82ea9875-694c-48e4-a734-59fbbf24a53d"",
            ""actions"": [
                {
                    ""name"": ""DeathBlow"",
                    ""type"": ""Button"",
                    ""id"": ""8e016d44-135e-4362-a7cf-5f77f7fc7817"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f153be49-7e2f-42ee-b929-f1bb6ce288e3"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""DeathBlow"",
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
        // Strikes
        m_Strikes = asset.FindActionMap("Strikes", throwIfNotFound: true);
        m_Strikes_Strikes = m_Strikes.FindAction("Strikes", throwIfNotFound: true);
        // Heavy
        m_Heavy = asset.FindActionMap("Heavy", throwIfNotFound: true);
        m_Heavy_Heavy = m_Heavy.FindAction("Heavy", throwIfNotFound: true);
        // DeathBlow
        m_DeathBlow = asset.FindActionMap("DeathBlow", throwIfNotFound: true);
        m_DeathBlow_DeathBlow = m_DeathBlow.FindAction("DeathBlow", throwIfNotFound: true);
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

    // Strikes
    private readonly InputActionMap m_Strikes;
    private IStrikesActions m_StrikesActionsCallbackInterface;
    private readonly InputAction m_Strikes_Strikes;
    public struct StrikesActions
    {
        private @SturdyMachineControls m_Wrapper;
        public StrikesActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Strikes => m_Wrapper.m_Strikes_Strikes;
        public InputActionMap Get() { return m_Wrapper.m_Strikes; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StrikesActions set) { return set.Get(); }
        public void SetCallbacks(IStrikesActions instance)
        {
            if (m_Wrapper.m_StrikesActionsCallbackInterface != null)
            {
                @Strikes.started -= m_Wrapper.m_StrikesActionsCallbackInterface.OnStrikes;
                @Strikes.performed -= m_Wrapper.m_StrikesActionsCallbackInterface.OnStrikes;
                @Strikes.canceled -= m_Wrapper.m_StrikesActionsCallbackInterface.OnStrikes;
            }
            m_Wrapper.m_StrikesActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Strikes.started += instance.OnStrikes;
                @Strikes.performed += instance.OnStrikes;
                @Strikes.canceled += instance.OnStrikes;
            }
        }
    }
    public StrikesActions @Strikes => new StrikesActions(this);

    // Heavy
    private readonly InputActionMap m_Heavy;
    private IHeavyActions m_HeavyActionsCallbackInterface;
    private readonly InputAction m_Heavy_Heavy;
    public struct HeavyActions
    {
        private @SturdyMachineControls m_Wrapper;
        public HeavyActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Heavy => m_Wrapper.m_Heavy_Heavy;
        public InputActionMap Get() { return m_Wrapper.m_Heavy; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HeavyActions set) { return set.Get(); }
        public void SetCallbacks(IHeavyActions instance)
        {
            if (m_Wrapper.m_HeavyActionsCallbackInterface != null)
            {
                @Heavy.started -= m_Wrapper.m_HeavyActionsCallbackInterface.OnHeavy;
                @Heavy.performed -= m_Wrapper.m_HeavyActionsCallbackInterface.OnHeavy;
                @Heavy.canceled -= m_Wrapper.m_HeavyActionsCallbackInterface.OnHeavy;
            }
            m_Wrapper.m_HeavyActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Heavy.started += instance.OnHeavy;
                @Heavy.performed += instance.OnHeavy;
                @Heavy.canceled += instance.OnHeavy;
            }
        }
    }
    public HeavyActions @Heavy => new HeavyActions(this);

    // DeathBlow
    private readonly InputActionMap m_DeathBlow;
    private IDeathBlowActions m_DeathBlowActionsCallbackInterface;
    private readonly InputAction m_DeathBlow_DeathBlow;
    public struct DeathBlowActions
    {
        private @SturdyMachineControls m_Wrapper;
        public DeathBlowActions(@SturdyMachineControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @DeathBlow => m_Wrapper.m_DeathBlow_DeathBlow;
        public InputActionMap Get() { return m_Wrapper.m_DeathBlow; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DeathBlowActions set) { return set.Get(); }
        public void SetCallbacks(IDeathBlowActions instance)
        {
            if (m_Wrapper.m_DeathBlowActionsCallbackInterface != null)
            {
                @DeathBlow.started -= m_Wrapper.m_DeathBlowActionsCallbackInterface.OnDeathBlow;
                @DeathBlow.performed -= m_Wrapper.m_DeathBlowActionsCallbackInterface.OnDeathBlow;
                @DeathBlow.canceled -= m_Wrapper.m_DeathBlowActionsCallbackInterface.OnDeathBlow;
            }
            m_Wrapper.m_DeathBlowActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DeathBlow.started += instance.OnDeathBlow;
                @DeathBlow.performed += instance.OnDeathBlow;
                @DeathBlow.canceled += instance.OnDeathBlow;
            }
        }
    }
    public DeathBlowActions @DeathBlow => new DeathBlowActions(this);
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
    public interface IStrikesActions
    {
        void OnStrikes(InputAction.CallbackContext context);
    }
    public interface IHeavyActions
    {
        void OnHeavy(InputAction.CallbackContext context);
    }
    public interface IDeathBlowActions
    {
        void OnDeathBlow(InputAction.CallbackContext context);
    }
}
