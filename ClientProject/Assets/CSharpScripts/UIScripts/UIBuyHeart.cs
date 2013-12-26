using UnityEngine;
using System.Collections;
using System;

public class UIBuyHeart : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
    }
    public override void OnShow()
    {
        base.OnShow();
    }

    private void OnConfirmClicked()
    {
		HideWindow();
		--GlobalVars.Coins;
		PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
		
		GA.API.Business.NewEvent("BuyHearts", "RMB", 1);
		
		GlobalVars.HeartCount = 5;
		UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
    }

    private void OnCancelClicked()
    {
        HideWindow();
		UIWindowManager.Singleton.GetUIWindow<UIStageInfo>().ShowWindow();
    }
}
