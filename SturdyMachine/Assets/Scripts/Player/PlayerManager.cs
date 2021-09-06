using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    OffenseManager _offenseManager;

    [SerializeField]
    GameObject _weaponGameObject;
    
    Animator _animator;
    SturdyMachineControls _sturdyMachineControl;

    Player _player;

    bool _isStanceActivated;

    OffenseDirection _currentOffenseDirection;
    OffenseType _currentOffenseType;

    void Awake() 
    {
        _sturdyMachineControl = new SturdyMachineControls();
        _animator = GetComponent<Animator>();

        _player = new Player(_animator, _offenseManager, new FusionWeapon(_weaponGameObject.GetComponent<MeshRenderer>(), _weaponGameObject.GetComponent<BoxCollider>(), _weaponGameObject.GetComponent<Rigidbody>(), _weaponGameObject.GetComponentInChildren<ParticleSystem>()));

        #region Strikes

        _sturdyMachineControl.Strikes.Strikes.performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                if (!_isStanceActivated)
                    _isStanceActivated = true;
            }
            else if (_isStanceActivated)
                _isStanceActivated = false;

            if (_isStanceActivated)
            {
                if (_currentOffenseDirection != OffenseDirection.STANCE)
                    _currentOffenseDirection = OffenseDirection.STANCE;

                if (_currentOffenseType != OffenseType.STRIKE)
                    _currentOffenseType = OffenseType.STRIKE;
            }
        };

        _sturdyMachineControl.Strikes.Strikes.canceled += context =>
        {
            if (_isStanceActivated)
                _isStanceActivated = false;
        };

        #endregion

        #region Heavy

        _sturdyMachineControl.Heavy.Heavy.performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                if (!_isStanceActivated)
                    _isStanceActivated = true;
            }
            else if (_isStanceActivated)
                _isStanceActivated = false;

            if (_isStanceActivated)
            {
                if (_currentOffenseDirection != OffenseDirection.STANCE)
                    _currentOffenseDirection = OffenseDirection.STANCE;

                if (_currentOffenseType != OffenseType.HEAVY)
                    _currentOffenseType = OffenseType.HEAVY;
            }
        };

        _sturdyMachineControl.Heavy.Heavy.canceled += context =>
        {
            if (_isStanceActivated)
                _isStanceActivated = false;
        };

        #endregion

        #region DeathBlow

        _sturdyMachineControl.DeathBlow.DeathBlow.performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                if (!_isStanceActivated)
                    _isStanceActivated = true;
            }
            else if (_isStanceActivated)
                _isStanceActivated = false;

            if (_isStanceActivated)
            {
                if (_currentOffenseDirection != OffenseDirection.STANCE)
                    _currentOffenseDirection = OffenseDirection.STANCE;

                if (_currentOffenseType != OffenseType.DEATHBLOW)
                    _currentOffenseType = OffenseType.DEATHBLOW;
            }
        };

        _sturdyMachineControl.DeathBlow.DeathBlow.canceled += context =>
        {
            if (_isStanceActivated)
                _isStanceActivated = false;

        };

        #endregion

        /*---DEFLECTION---*/

        //Neutral
        _sturdyMachineControl.Deflection.Neutral.performed += context =>
        {
            if (context.interaction is PressInteraction)
            {
                _currentOffenseDirection = OffenseDirection.NEUTRAL;

                if (!_isStanceActivated)
                {
                    if (_currentOffenseType != OffenseType.DEATHBLOW)
                        _currentOffenseType = OffenseType.DEFLECTION;
                }
            }
        };

        //Right
        _sturdyMachineControl.Deflection.Right.performed += context =>
        {
            if (context.interaction is PressInteraction)
            {
                if (_currentOffenseDirection != OffenseDirection.RIGHT)
                    _currentOffenseDirection = OffenseDirection.RIGHT;

                if (!_isStanceActivated)
                {
                    if (_currentOffenseType != OffenseType.DEFLECTION)
                        _currentOffenseType = OffenseType.DEFLECTION;
                }
            }
        };

        //Left
        _sturdyMachineControl.Deflection.Left.performed += context =>
        {
            if (context.interaction is PressInteraction)
            {
                if (_currentOffenseDirection != OffenseDirection.LEFT)
                    _currentOffenseDirection = OffenseDirection.LEFT;

                if (!_isStanceActivated)
                {
                    if (_currentOffenseType != OffenseType.DEFLECTION)
                        _currentOffenseType = OffenseType.DEFLECTION;
                }
            };
        };

        //Evasion
        _sturdyMachineControl.Deflection.Evasion.performed += context =>
        {
            if (context.interaction is PressInteraction)
            {
                if (_isStanceActivated)
                {
                    if (_currentOffenseDirection != OffenseDirection.NEUTRAL)
                        _currentOffenseDirection = OffenseDirection.NEUTRAL;

                    if (_currentOffenseType != OffenseType.SWEEP)
                        _currentOffenseType = OffenseType.SWEEP;

                }
                else 
                {
                    if (_currentOffenseDirection != OffenseDirection.EVASION)
                        _currentOffenseDirection = OffenseDirection.EVASION;

                    if (_currentOffenseType != OffenseType.DEFLECTION)
                        _currentOffenseType = OffenseType.DEFLECTION;
                }
            }
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        _player.Start();

        _currentOffenseDirection = OffenseDirection.STANCE;
        _currentOffenseType = OffenseType.DEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        _player.Update(_currentOffenseDirection, _currentOffenseType, _isStanceActivated);

        if (!_isStanceActivated) 
        {
            if (_currentOffenseDirection != OffenseDirection.STANCE)
                _currentOffenseDirection = OffenseDirection.STANCE;

            if (_currentOffenseType != OffenseType.DEFAULT)
                _currentOffenseType = OffenseType.DEFAULT;
        }
    }

    private void LateUpdate()
    {
        _player.LateUpdate();
    }

    private void OnEnable()
    {
        _sturdyMachineControl.Deflection.Enable();
        _sturdyMachineControl.Sweep.Enable();
        _sturdyMachineControl.Strikes.Enable();
        _sturdyMachineControl.Heavy.Enable();
        _sturdyMachineControl.DeathBlow.Enable();
    }

    private void OnDisable()
    {
        _sturdyMachineControl.Deflection.Disable();
        _sturdyMachineControl.Sweep.Disable();
        _sturdyMachineControl.Strikes.Disable();
        _sturdyMachineControl.Heavy.Disable();
        _sturdyMachineControl.DeathBlow.Disable();
    }
}