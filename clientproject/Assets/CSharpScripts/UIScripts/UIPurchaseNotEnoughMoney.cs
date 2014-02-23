using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNotEnoughMoney : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
    UISprite m_itemIcon;
	public delegate void OnPurchaseFunc();
	
	public OnPurchaseFunc OnPurchase; 
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("GetCoinBtn", delegate()
		{
			HideWindow();
			UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.140coins");
            });
		});
        AddChildComponentMouseClick("ShopBtn", delegate()
		{
			HideWindow(delegate()
			{
				UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
                UIWindowManager.Singleton.GetUIWindow<UIStore>().OnPurchaseFunc = delegate()
                {
                    //打开使用窗口
                    if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_Hammer)
                    {
                        UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>();
                        uiWindow.ShowWindow();
                    }
                    else
                    {
                        UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>();
                        uiWindow.ShowWindow();
                    }
                };
			});
		});

        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow(delegate()
            {
                if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
                {
                    UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
                }
            });
        });

		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
        m_itemIcon = GetChildComponent<UISprite>("ItemIcon");
    }
    public override void OnShow()
    {
        base.OnShow();

        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString() + "/" + Unibiller.GetCurrencyBalance("gold");
        m_msgLabel.text = Localization.instance.Get("Intro_" + GlobalVars.UsingItem.ToString());
        m_itemIcon.spriteName = GlobalVars.UsingItem.ToString();
    }
}
