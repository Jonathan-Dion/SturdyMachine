using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

abstract class Hunanoid
{
    protected Animator _animator;
    protected FusionWeapon _fusionWeapon;
    protected OffenseManager _offenseManager;

    public abstract FusionWeapon GetFusionWeapon { get; }
    public abstract Animator GetAnimator { get; }
    public abstract OffenseManager GetOffenseManager { get; }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update(OffenseDirection pOffenseDirection, OffenseType pOffenseType, bool pIsStance) { }
    public virtual void LateUpdate() { }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(Hunanoid))]
//public class HumanoidEditor : Editor 
//{
    
//}


//#endif