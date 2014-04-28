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
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem2Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.130coins");
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem3Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.420coins");
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem4Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.1040coins");
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        AddChildComponentMouseClick("BuyItem5Btn", delegate()
        {
            HideWindow();
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.2080coins");
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
    }

    public void OnCloseBtn()
    {
        HideWindow();
        if (GlobalVars.OnCancelFunc != null)
            GlobalVars.OnCancelFunc();				//完成后执行的函数
    }
}
