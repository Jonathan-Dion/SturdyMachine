using UnityEngine;

abstract class Hunanoid
{
    protected Animator _animator;

    protected OffenseManager _offenseManager;

    public abstract Animator GetAnimator { get; }
    public abstract OffenseManager GetOffenseManager { get; }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) { }
}