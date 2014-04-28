using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNotEnoughMoney : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
    UISprite m_itemIcon;
    UISprite m_buyItemBtnSprite;

    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("GetCoinBtn", delegate()
		{
			HideWindow();
			UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                int coinNeeded = CapsConfig.GetItemPrice(GlobalVars.UsingItem) - (int)Unibiller.GetCurrencyBalance("gold");
                if (coinNeeded <= 60)
                {
                    Unibiller.initiatePurchase("com.linkrstudio.jellycraft.60coins");
                }
                else
                {
                    Unibiller.initiatePurchase("com.linkrstudio.jellycraft.130coins");
                }
                
                UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
                UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
            });
		});
        AddChildComponentMouseClick("ShopBtn", delegate()
		{
			HideWindow(delegate()
			{
                UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
			});
		});

        AddChildComponentMouseClick("CloseBtn", OnCloseBtn);

		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
        m_itemIcon = GetChildComponent<UISprite>("ItemIcon");
        m_buyItemBtnSprite = GetChildComponent<UISprite>("GetCoinTextSprite");
    }

    public void OnCloseBtn()
    {
        HideWindow(delegate()
        {
			if (GlobalVars.OnCancelFunc != null)
            {
				GlobalVars.OnCancelFunc();
            }
        });
    }

    public override void OnShow()
    {
        base.OnShow();
        UIStageInfo stageUI = UIWindowManager.Singleton.GetUIWindow<UIStageInfo>();
        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString() + "/" + (Unibiller.GetCurrencyBalance("gold") - stageUI.GetCurCost()).ToString();
        m_msgLabel.text = Localization.instance.Get("Intro_" + GlobalVars.UsingItem.ToString());
        m_itemIcon.spriteName = GlobalVars.UsingItem.ToString();

        if (CapsConfig.GetItemPrice(GlobalVars.UsingItem) <= 60)
        {
            m_buyItemBtnSprite.spriteName = "Button_Buy60Coins";
        }
        else
        {
            m_buyItemBtnSprite.spriteName = "Button_Buy130Coins";
        }
    }

    void OnPurchased(PurchasableItem item)      //购买成功
    {
        Debug.Log("Purchase Succeed");
        GlobalVars.PurchaseSuc = true;
    }

    void OnPurchaseFailed(PurchasableItem item)     //购买失败
    {
        Debug.Log("Purchase Failed");
        GlobalVars.PurchaseFailed = true;
    }

    void OnPurchaseCancelled(PurchasableItem item)      //主动取消购买
    {
        Debug.Log("Purchase Cancelled");
        GlobalVars.PurchaseCancel = true;
    }
}
