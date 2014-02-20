using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNotEnoughMoney : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
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
    }
    public override void OnShow()
    {
        base.OnShow();

		if(GlobalVars.UsingItem == PurchasedItem.Item_Hammer)
		{
			m_costLabel.text = "6/" + Unibiller.GetCurrencyBalance("gold");
			m_msgLabel.text = Localization.instance.Get("Intro_Hammer");
		}
		
		if(GlobalVars.UsingItem == PurchasedItem.Item_PlusStep)
		{
			m_costLabel.text = "6/" + Unibiller.GetCurrencyBalance("gold");
			m_msgLabel.text = Localization.instance.Get("Intro_StepPlus5");
		}
    }
}
