//class ToolBox 
//{
//    public static bool Offset(ref float pCurrentCooldown, ref float pMaxCooldown) 
//    {
//        pCurrentCooldown += UnityEngine.Time.deltaTime;

//        if (pCurrentCooldown >= pMaxCooldown) 
//        {
//            pCurrentCooldown = 0f;
//            pMaxCooldown = 0f;

//            return false;
//        }

//        return true;
//    }

//    public static void Bypass(UnityEngine.Animator pAnimator, CustomAnimation[] pCustomAnimation, CustomAnimation pNextCustomAnimation, ref float pMaxCooldown)
//    {
//        /*if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != pNextCustomAnimation.name)
//        {
            
//        }*/

//        if (pNextCustomAnimation.canBypassed)
//        {
//            CustomAnimation currentCustomAnimation = GetCurrentCustomAnimation(pAnimator, pCustomAnimation);
//            bool isBypassed = GetIsBypassed(pNextCustomAnimation, currentCustomAnimation);

//            if (isBypassed)
//            {
//                pAnimator.Play(pNextCustomAnimation.name);

//                CooldownInit(pNextCustomAnimation, ref pMaxCooldown);
//            }
//        }
//    }

//    public static void SetAnimation(UnityEngine.Animator pAnimator, CustomAnimation[] pCustomAnimation, ActionMap pActionMap, Action pAction, ref bool pIsCooldown, ref float pMaxCooldown) 
//    {
//        foreach (CustomAnimation customAnimation in pCustomAnimation)
//        {
//            if (!pIsCooldown)
//            {
//                if (GetCurrentCustomAnimation(pAnimator, pCustomAnimation).name.Contains("Idle"))
//                {
//                    if (pMaxCooldown == 0)
//                    {
//                        if (customAnimation.actionMapType == pActionMap)
//                        {
//                            if (customAnimation.actionType == pAction)
//                            {
//                                pAnimator.Play(customAnimation.name);
//                                CooldownInit(customAnimation, ref pMaxCooldown);
//                            }
//                        }
//                    }
//                }
//                else
//                    break;
//            }
//        }

//        //Bypass
//        if (GetCurrentCustomAnimation(pAnimator, pCustomAnimation).name.Contains("Idle"))
//        {
//            Bypass(pAnimator, pCustomAnimation, GetNextCustomAnimation(pCustomAnimation, pActionMap, pAction), ref pMaxCooldown);
//        }
//    }

//    public static void LateUpdate(UnityEngine.Animator pAnimator, CustomAnimation[] pCustomAnimation, ref UnityEngine.GameObject pTrailSmall, ref float pMaxCooldown, ref bool pIsCooldown, ref float pCurrentCooldown) 
//    {
//        if (pMaxCooldown != 0) 
//        {
//            if (GetCurrentCustomAnimation(pAnimator, pCustomAnimation).name != null)
//                if (GetCurrentCustomAnimation(pAnimator, pCustomAnimation).name.Contains("Idle"))
//                    pIsCooldown = true;

//            if (pIsCooldown)
//                pIsCooldown = Offset(ref pCurrentCooldown, ref pMaxCooldown);
//        }

//        if (GetCurrentCustomAnimation(pAnimator, pCustomAnimation).name.Contains("Idle"))
//        {
//            if (pTrailSmall.activeSelf)
//                pTrailSmall.SetActive(false);
//        }
//        else if (!pTrailSmall.activeSelf)
//            pTrailSmall.SetActive(true);
//    }

//    public static void Initialization(CustomAnimation[] pCustomAnimation) 
//    {
//        for (int i = 0; i < pCustomAnimation.Length; ++i)
//        {
//            //Initialize animation name
//            pCustomAnimation[i].name = pCustomAnimation[i].animationClip.name;

//            if (pCustomAnimation[i].canBypassed) 
//            {
//                //Initialize cancel animation name
//                for (int j = 0; j < pCustomAnimation[i].cancel.Length; j++)
//                {
//                    //Initialize animation name size
//                    if (pCustomAnimation[i].cancelName.Length != pCustomAnimation[i].cancel.Length)
//                        pCustomAnimation[i].cancelName = new string[pCustomAnimation[i].cancel.Length];

//                    pCustomAnimation[i].cancelName[j] = pCustomAnimation[i].cancel[j].name;
//                }
//            }
//        }
//    }

//    public static CustomAnimation GetCurrentCustomAnimation(UnityEngine.Animator pAnimator, CustomAnimation[] pCustomAnimation)
//    {
//        for (int i = 0; i < pCustomAnimation.Length; ++i)
//        {
//            if (pAnimator.GetCurrentAnimatorClipInfo(0).Length != 0) 
//            {
//                if (pAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == pCustomAnimation[i].name)
//                    return pCustomAnimation[i];
//            }
//        }

//        CustomAnimation currentAnimation = new CustomAnimation();

//        return currentAnimation;
//    }

//    public static CustomAnimation GetNextCustomAnimation(CustomAnimation[] pCustomAnimation, ActionMap pActionMap, Action pAction) 
//    {
//        CustomAnimation nextCustomAnimation = new CustomAnimation();

//        for (int i = 0; i < pCustomAnimation.Length; ++i)
//            if (pCustomAnimation[i].actionMapType == pActionMap)
//                if (pCustomAnimation[i].actionType == pAction)
//                    nextCustomAnimation = pCustomAnimation[i];

//        return nextCustomAnimation;
//    }

//    public static bool GetIsBypassed(CustomAnimation pNextCustomAnimation, CustomAnimation pCurrentCustomAnimation)
//    {
//        bool isBypassed = false;

//        for (int i = 0; i < pNextCustomAnimation.cancelName.Length; ++i)
//        {
//            if (pNextCustomAnimation.cancelName[i] == pCurrentCustomAnimation.name)
//            {
//                isBypassed = true;
//                break;
//            }
//        }

//        return isBypassed;
//    }

//    static void CooldownInit(CustomAnimation pCustomAnimation, ref float pMaxCooldown) 
//    {
//        if (pCustomAnimation.cooldown != 0)
//            pMaxCooldown = pCustomAnimation.cooldown;
//    }
//}