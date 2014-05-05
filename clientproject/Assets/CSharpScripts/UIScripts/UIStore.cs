using UnityEngine;
using System.Collections;
using System;

public class UIStore : UIWindow 
{
    GameObject m_disableBtn;

    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", OnCloseBtn);

        AddChildComponentMouseClick("BuyItem1Btn", delegate()
        {
            HideWindow();
			Unibiller.initiatePurchase("com.linkrstudio.jellycraft.60coins");
            GlobalVars.PurchasingItemName = "60Coins";
            GlobalVars.PurchasingItemPrice = 99;
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem2Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.130coins");
            GlobalVars.PurchasingItemName = "130Coins";
            GlobalVars.PurchasingItemPrice = 199;
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem3Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.360coins");
            GlobalVars.PurchasingItemName = "360Coins";
            GlobalVars.PurchasingItemPrice = 499;
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem4Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.860coins");
            GlobalVars.PurchasingItemName = "860Coins";
            GlobalVars.PurchasingItemPrice = 999;
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem5Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.1680coins");
            GlobalVars.PurchasingItemName = "1680Coins";
            GlobalVars.PurchasingItemPrice = 1999;
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        m_disableBtn = UIToolkits.FindChild(mUIObject.transform, "DisableBtn").gameObject;
    }

    public override void OnShow()
    {
        base.OnShow();
        int cost = CapsConfig.GetItemPrice(GlobalVars.UsingItem);
        UIButton btn = GetChildComponent<UIButton>("BuyItem1Btn");
        if (cost > 60)      //要用的道具高于60金币，屏蔽买60金币的按钮
        {
            btn.enabled = false;
            m_disableBtn.SetActive(true);
        }
        else
        {
            btn.enabled = true;
            m_disableBtn.SetActive(false);
        }
    }

    public void OnCloseBtn()
    {
        HideWindow();
        if (GlobalVars.OnCancelFunc != null)
            GlobalVars.OnCancelFunc();				//完成后执行的函数
    }
}
