using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNoTarget : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
	public delegate void OnPurchaseFunc();
	
	public OnPurchaseFunc OnPurchase; 
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
    }
    public override void OnShow()
    {
        base.OnShow();
        //GameLogic.Singleton.PauseGame();
		if(GlobalVars.UsingItem == PurchasedItem.Item_Hammer)
		{
			
		}
		
		if(GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
		{
			
		}
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            //GameLogic.Singleton.ResumeGame();
        });
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
        {
            if (Unibiller.DebitBalance("gold", 6))      //花钱
            {
                GA.API.Business.NewEvent("BuyStep", "RMB", 1);
                PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
                if (GlobalVars.CurStageData.StepLimit > 0)
                    GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
                else if (GlobalVars.CurStageData.TimeLimit > 0)
                    GameLogic.Singleton.AddGameTime(15);        //Add 15 Seconds
            }
        }
    }

    public void OnCancelClicked()
    {
        HideWindow(delegate()
        {
            //GameLogic.Singleton.ResumeGame();
        });
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
        {
            if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
            }
        }
    }
}
