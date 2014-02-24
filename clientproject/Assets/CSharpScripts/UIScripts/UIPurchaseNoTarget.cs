using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNoTarget : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
    UISprite m_itemIcon;

    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
        m_itemIcon = GetChildComponent<UISprite>("ItemIcon");
    }
    public override void OnShow()
    {
        base.OnShow();
        //GameLogic.Singleton.PauseGame();
        
        //获取道具信息
        m_msgLabel.text = Localization.instance.Get("Use_" + GlobalVars.UsingItem.ToString());
        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString();
        m_itemIcon.spriteName = GlobalVars.UsingItem.ToString();

        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(true);
    }

    public override void OnHideEffectPlayOver()
    {
        base.OnHideEffectPlayOver();
        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(false);
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            //GameLogic.Singleton.ResumeGame();

            if (Unibiller.DebitBalance("gold", CapsConfig.GetItemPrice(GlobalVars.UsingItem)))      //花钱
            {
                GA.API.Business.NewEvent(GlobalVars.UsingItem.ToString(), "Coins", CapsConfig.GetItemPrice(GlobalVars.UsingItem));
                if (GlobalVars.UsingItem == PurchasedItem.ItemAfterGame_PlusStep)
                {
                    GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
                    GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);      //回到可以继续玩的状态
                    GameLogic.Singleton.ResumeGame();
                }
                if (GlobalVars.UsingItem == PurchasedItem.ItemAfterGame_PlusTime)
                {
                    GameLogic.Singleton.SetGameTime(15);        //Add 15 Seconds
                    GameLogic.Singleton.SetGameFlow(TGameFlow.EGameState_Playing);      //回到可以继续玩的状态
                    GameLogic.Singleton.ResumeGame();
                }
                
                if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_Resort)            //重排道具
                {
                    GameLogic.Singleton.AutoResort();           //自动重排
                }

                if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_ChocoStoper)       //停止巧克力
                {
                    GameLogic.Singleton.UseStopChocoItem();
                }

                if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_TimeStoper)        //时间暂停
                {
                    GameLogic.Singleton.UserStopTimeItem();
                }
            }
        });
    }

    public void OnCancelClicked()
    {
        HideWindow(delegate()
        {
            //GameLogic.Singleton.ResumeGame();
            if (GlobalVars.UsingItem == PurchasedItem.ItemAfterGame_PlusStep ||
            GlobalVars.UsingItem == PurchasedItem.ItemAfterGame_PlusTime)
            {
                UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
            }
        });
    }
}
