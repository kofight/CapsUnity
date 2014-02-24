using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNotEnoughMoney : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
    UISprite m_itemIcon;

    public UIWindow.WindowEffectFinished OnPurchaseFunc;
    public UIWindow.WindowEffectFinished OnCancelFunc;
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("GetCoinBtn", delegate()
		{
			HideWindow();
			UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
				Unibiller.onPurchaseComplete += OnPurchased;
                Unibiller.onPurchaseFailed += OnPurchaseFailed;
                Unibiller.onPurchaseCancelled += OnPurchaseCancelled;
				
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.140coins");
            });
		});
        AddChildComponentMouseClick("ShopBtn", delegate()
		{
			HideWindow(delegate()
			{
				UIWindowManager.Singleton.GetUIWindow<UIStore>().ShowWindow();
                UIWindowManager.Singleton.GetUIWindow<UIStore>().OnPurchaseFunc = OnPurchaseFunc;
                UIWindowManager.Singleton.GetUIWindow<UIStore>().OnCancelFunc = OnCancelFunc;
			});
		});

        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow(delegate()
            {
                if (OnCancelFunc != null)
                {
                    OnCancelFunc();
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
        UIStageInfo stageUI = UIWindowManager.Singleton.GetUIWindow<UIStageInfo>();
        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString() + "/" + (Unibiller.GetCurrencyBalance("gold") - stageUI.GetCurCost()).ToString();
        m_msgLabel.text = Localization.instance.Get("Intro_" + GlobalVars.UsingItem.ToString());
        m_itemIcon.spriteName = GlobalVars.UsingItem.ToString();
    }

    void OnPurchased(PurchasableItem item)      //购买成功
    {
        Debug.Log("Purchase Succeed");
        UnregisterPurchase();

        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();

        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnPurchaseFunc);        //设置完成后执行的函数
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
    }

    void UnregisterPurchase()
    {
        Unibiller.onPurchaseComplete -= OnPurchased;
        Unibiller.onPurchaseFailed -= OnPurchaseFailed;
        Unibiller.onPurchaseCancelled -= OnPurchaseCancelled;
    }

    void OnPurchaseFailed(PurchasableItem item)     //购买失败
    {
        Debug.Log("Purchase Failed");

        UnregisterPurchase();
        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
        
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseFailed"));
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnCancelFunc);        //设置完成后执行的函数
    }

    void OnPurchaseCancelled(PurchasableItem item)      //主动取消购买
    {
        Debug.Log("Purchase Cancelled");

        UnregisterPurchase();
        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
        
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseCancelled"));
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnCancelFunc);        //设置完成后执行的函数
    }
}
