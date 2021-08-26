//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Interactions;

//public class AnimationController : MonoBehaviour
//{
    

//    SturdyMachineControls _sturdyMachineControl;

//    [SerializeField]
//    CustomAnimation[] _customAnimation;

//    [SerializeField]
//    GameObject _trailSmall;

//    float _currentCooldown, _maxCooldown;
//    bool _isCooldown, _isHoldInteraction;
//    int _idleStrikeHash, _idleHeavyHash, _idleDeathblowHash;

//    Action _currentAction;
//    ActionMap _currentActionMap;

//    void Awake()
//    {
//        _idleStrikeHash = Animator.StringToHash("Strikes");
//        _idleHeavyHash = Animator.StringToHash("Heavy");
//        _idleDeathblowHash = Animator.StringToHash("DeathBlow");

//        ToolBox.Initialization(_customAnimation);

//        _sturdyMachineControl = new SturdyMachineControls();

//        #region Strikes
//        _sturdyMachineControl.Strikes.Strikes.performed += context =>
//        {
//            _isHoldInteraction = (context.interaction is HoldInteraction) ? true : false;

//            if (_isHoldInteraction)
//            {
//                if (_currentActionMap != ActionMap.STRIKE)
//                    _currentActionMap = ActionMap.STRIKE;

//                if (_currentAction != Action.DEFAULT)
//                    _currentAction = Action.DEFAULT;

//                _animator.SetTrigger(_idleStrikeHash);
//                ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//            }
//        };

//        _sturdyMachineControl.Strikes.Strikes.canceled += context =>
//        {
//            _isHoldInteraction = false;
//            _currentActionMap = ActionMap.DEFAULT;

//            _animator.ResetTrigger(_idleStrikeHash);
//        };
//        #endregion

//        #region Heavy
//        _sturdyMachineControl.Heavy.Heavy.performed += context =>
//        {
//            _isHoldInteraction = (context.interaction is HoldInteraction) ? true : false;

//            if (_isHoldInteraction)
//            {
//                if (_currentActionMap != ActionMap.HEAVY)
//                    _currentActionMap = ActionMap.HEAVY;

//                if (_currentAction != Action.DEFAULT)
//                    _currentAction = Action.DEFAULT;

//                _animator.SetTrigger(_idleHeavyHash);
//                ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//            }
//        };

//        _sturdyMachineControl.Heavy.Heavy.canceled += context =>
//        {
//            _isHoldInteraction = false;
//            _currentActionMap = ActionMap.DEFAULT;

//            _animator.ResetTrigger(_idleHeavyHash);
//        };
//        #endregion

//        #region DeathBlow
//        /*---DEATHBLOW---*/
//        _sturdyMachineControl.DeathBlow.DeathBlow.performed += context =>
//        {
//            _isHoldInteraction = (context.interaction is HoldInteraction) ? true : false;

//            if (_isHoldInteraction)
//            {
//                if (_currentActionMap != ActionMap.DEATHBLOW)
//                    _currentActionMap = ActionMap.DEATHBLOW;

//                if (_currentAction != Action.DEFAULT)
//                    _currentAction = Action.DEFAULT;

//                _animator.SetTrigger(_idleDeathblowHash);
//                ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//            }
//        };

//        _sturdyMachineControl.DeathBlow.DeathBlow.canceled += context =>
//        {
//            _isHoldInteraction = false;
//            _currentActionMap = ActionMap.DEFAULT;

//            _animator.ResetTrigger(_idleDeathblowHash);
//        };
//        #endregion

//        /*---DEFLECTION---*/

//        //Neutral
//        _sturdyMachineControl.Deflection.Neutral.performed += context =>
//        {
//            if (context.interaction is PressInteraction)
//            {
//                _currentAction = Action.NEUTRAL;

//                if (_isHoldInteraction)
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//                else
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, ActionMap.DEFLECTION, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//            }
//        };

//        //Right
//        _sturdyMachineControl.Deflection.Right.performed += context =>
//        {
//            if (context.interaction is PressInteraction)
//            {
//                _currentAction = Action.RIGHT;

//                if (_isHoldInteraction)
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//                else
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, ActionMap.DEFLECTION, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//            }
//        };

//        //Left
//        _sturdyMachineControl.Deflection.Left.performed += context =>
//        {
//            if (context.interaction is PressInteraction)
//            {
//                _currentAction = Action.LEFT;

//                if (_isHoldInteraction)
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, _currentActionMap, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//                else
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, ActionMap.DEFLECTION, _currentAction, ref _isCooldown, ref _maxCooldown);
//                }
//            }
//        };

//        //Evasion
//        _sturdyMachineControl.Deflection.Evasion.performed += context =>
//        {
//            if (context.interaction is PressInteraction)
//            {
//                if (_isHoldInteraction)
//                {
//                    if (_currentActionMap == ActionMap.STRIKE)
//                        ToolBox.SetAnimation(_animator, _customAnimation, ActionMap.SWEEP, Action.NEUTRAL, ref _isCooldown, ref _maxCooldown);
//                }
//                else
//                {
//                    ToolBox.SetAnimation(_animator, _customAnimation, ActionMap.DEFLECTION, Action.EVASION, ref _isCooldown, ref _maxCooldown);
//                }
//            }
//        };
//    }

//    private void OnEnable()
//    {
//        _sturdyMachineControl.Deflection.Enable();
//        _sturdyMachineControl.Sweep.Enable();
//        _sturdyMachineControl.Strikes.Enable();
//        _sturdyMachineControl.Heavy.Enable();
//        _sturdyMachineControl.DeathBlow.Enable();
//    }

//    private void OnDisable()
//    {
//        _sturdyMachineControl.Deflection.Disable();
//        _sturdyMachineControl.Sweep.Disable();
//        _sturdyMachineControl.Strikes.Disable();
//        _sturdyMachineControl.Heavy.Disable();
//        _sturdyMachineControl.DeathBlow.Disable();
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        _animator = GetComponent<Animator>();
//    }

//    void LateUpdate()
//    {
//        ToolBox.LateUpdate(_animator, _customAnimation, ref _trailSmall, ref _maxCooldown, ref _isCooldown, ref _currentCooldown);
//    }

//    private void OnGUI()
//    {
//        GUI.Label(new Rect(0, 0, 300, 300), (_isCooldown) ? "Cooldown: Activated \t " + _currentCooldown.ToString("#.00") + "/" + _maxCooldown : "Cooldown: desactivated");
//    }
//}