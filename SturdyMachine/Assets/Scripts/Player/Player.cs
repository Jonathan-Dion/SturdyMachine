using UnityEngine;

class Player : Hunanoid
{
    public override FusionWeapon GetFusionWeapon => _fusionWeapon;
    public override Animator GetAnimator => _animator;
    public override OffenseManager GetOffenseManager => _offenseManager;

    public Player() 
    {
        _fusionWeapon = new FusionWeapon();
    }

    public Player(Animator pAnimator, OffenseManager pOffenseManager, FusionWeapon pFusionWeapon) 
    {
        _animator = pAnimator;
        _offenseManager = pOffenseManager;

        _fusionWeapon = pFusionWeapon;
    }

    public override void Awake()
    {
        
    }

    public override void Start()
    {
        
    }

    public override void Update(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance)
    {
        _offenseManager.SetAnimation(_animator, pOffenseDirection, pOffenseType, pIsStance);
    }

    public override void LateUpdate()
    {
        _fusionWeapon.LateUpdate(_offenseManager.GetCurrentOffenseDirection);
    }

    //private void OnGUI()
    //{

    //    GUI.Label(new Rect(0, 0, 300, 300), (_offense.GetIsCooldownAvailable) ? "Cooldown: Activated \t " + _offense.GetCurrentCooldownTime.ToString("#.00") + "/" + _customAnimationManager.GetMaxCooldownTime : "Cooldown: desactivated");
    //}
}