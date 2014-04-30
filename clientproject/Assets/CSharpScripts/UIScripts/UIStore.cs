using UnityEngine;
using System.Collections;
using System;

public class UIStore : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("CloseBtn", OnCloseBtn);

        AddChildComponentMouseClick("BuyItem1Btn", delegate()
        {
            HideWindow();
			Unibiller.initiatePurchase("com.linkrstudio.jellycraft.60coins");
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem2Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.130coins");
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem3Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.360coins");
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem4Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.860coins");
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem5Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.1680coins");
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
    }

    public override void OnShow()
    {
        base.OnShow();
        int cost = CapsConfig.GetItemPrice(GlobalVars.UsingItem);
        UIButton btn = GetChildComponent<UIButton>("BuyItem1Btn");
        if (cost > 60)      //要用的道具高于60金币，屏蔽买60金币的按钮
        {
            btn.enabled = false;
        }
        else
        {
            btn.enabled = true;
        }
    }

    public void OnCloseBtn()
    {
        HideWindow();
        if (GlobalVars.OnCancelFunc != null)
            GlobalVars.OnCancelFunc();				//完成后执行的函数
    }
}
