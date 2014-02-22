using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNoTarget : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;

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
            m_costLabel.text = "70";
        }
        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
        {
            m_msgLabel.text = Localization.instance.Get("Use_AddTime");
            m_costLabel.text = "70";
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

            if (GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
            {
                if (Unibiller.DebitBalance("gold", 70))      //花钱
                {
                    GA.API.Business.NewEvent("BuyStep", "Coins", 70);
                    GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
                    GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);      //回到可以继续玩的状态
                    GameLogic.Singleton.ResumeGame();
                }
            }
            if (GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
            {
                if (Unibiller.DebitBalance("gold", 70))      //花钱
                {
                    GA.API.Business.NewEvent("BuyTime", "Coins", 70);
                    if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
                    {
                        GameLogic.Singleton.SetGameTime(15);        //Add 15 Seconds
                        GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);      //回到可以继续玩的状态
                        GameLogic.Singleton.ResumeGame();
                    }
                    else
                    {
                        GameLogic.Singleton.AddGameTime(15);        //Add 15 Seconds
                    }
                }
            }
        });
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
