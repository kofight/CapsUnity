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
			});
		});

        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow();
        });

		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
        m_itemIcon = GetChildComponent<UISprite>("ItemIcon");
    }
    public override void OnShow()
    {
        base.OnShow();

		if(GlobalVars.UsingItem == PurchasedItem.Item_Hammer)
		{
			m_costLabel.text = "6/" + Unibiller.GetCurrencyBalance("gold");
			m_msgLabel.text = Localization.instance.Get("Intro_Hammer");
            m_itemIcon.spriteName = "ItemHammer";
		}
		
		if(GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
		{
			m_costLabel.text = "6/" + Unibiller.GetCurrencyBalance("gold");
            m_msgLabel.text = Localization.instance.Get("Intro_PlusStep");
            m_itemIcon.spriteName = "ItemStep";
		}

        if (GlobalVars.UsingItem == PurchasedItem.Item_PlusTime)
        {
            m_costLabel.text = "6/" + Unibiller.GetCurrencyBalance("gold");
            m_msgLabel.text = Localization.instance.Get("Intro_PlusTime");
            m_itemIcon.spriteName = "ItemStep";
        }
    }
}
