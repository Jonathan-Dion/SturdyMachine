using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;

using SturdyMachine.Offense.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    OffenseManager _offenseManager;

    //[SerializeField]
    //FusionBlade _fusionBlade;
    
    Animator _animator;
    SturdyMachineControls _sturdyMachineControl;

    //Player _player;

    bool _isStanceActivated;

    OffenseDirection _currentOffenseDirection;
    OffenseType _currentOffenseType;

    void Awake() 
    {
        _sturdyMachineControl = new SturdyMachineControls();
        _animator = GetComponent<Animator>();

        //FusionWeapon fusionWeapon = new FusionWeapon(_weaponGameObject.GetComponent<MeshRenderer>(), _weaponGameObject.GetComponent<BoxCollider>(), _weaponGameObject.GetComponent<Rigidbody>(), _weaponGameObject.GetComponentInChildren<ParticleSystem>());

        //_player = new Player(_animator, _offenseManager, );

        

    }

    // Start is called before the first frame update
    void Start()
    {
        //_player.Start();

        _currentOffenseDirection = OffenseDirection.STANCE;
        _currentOffenseType = OffenseType.DEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        //_player.Update(_currentOffenseDirection, _currentOffenseType, _isStanceActivated);

        if (!_isStanceActivated)
        {
            if (_currentOffenseType != OffenseType.DEFAULT)
                _currentOffenseType = OffenseType.DEFAULT;
        }

        if (_currentOffenseDirection != OffenseDirection.STANCE)
            _currentOffenseDirection = OffenseDirection.STANCE;
    }

    private void LateUpdate()
    {
        //_player.LateUpdate();
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