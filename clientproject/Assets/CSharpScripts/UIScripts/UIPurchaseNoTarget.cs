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
		if(GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
		{
            m_msgLabel.text = Localization.instance.Get("Use_AddStep");
            m_costLabel.text = "6";
        }
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
        {
            m_msgLabel.text = Localization.instance.Get("Use_AddTime");
            m_costLabel.text = "6";
        }
        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(true);
    }

    public override void OnHide()
    {
        base.OnHide();
        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(false);
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
                GA.API.Business.NewEvent("BuyStep", "Coins", 6);
                GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
            }
        }
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
        {
            if (Unibiller.DebitBalance("gold", 6))      //花钱
            {
                GA.API.Business.NewEvent("BuyTime", "Coins", 6);
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
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusStep ||
            GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
        {
            if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
            }
        }
    }
}
