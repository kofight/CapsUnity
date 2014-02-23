using UnityEngine;
using System.Collections;
using System;

public class UIStore : UIWindow 
{
    public UIWindow.WindowEffectFinished OnPurchaseFunc;
    public UIWindow.WindowEffectFinished OnCancelFunc;

    public override void OnCreate()
    {
        base.OnCreate();
        //AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        //AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnCancelFunc);        //设置完成后执行的函数
        });

        AddChildComponentMouseClick("BuyItem1Btn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.140coins");
            });
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
        AddChildComponentMouseClick("BuyItem2Btn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.800coins");
            });
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
        AddChildComponentMouseClick("BuyItem3Btn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.1700coins");
            });
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
        AddChildComponentMouseClick("BuyItem4Btn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.4500coins");
            });
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });
        AddChildComponentMouseClick("BuyItem5Btn", delegate()
        {
            HideWindow();
            UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow(delegate()
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.9500coins");
            });
            UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));
        });

        Unibiller.onPurchaseComplete += OnPurchased;
        Unibiller.onPurchaseFailed += OnPurchaseFailed;
        Unibiller.onPurchaseCancelled += OnPurchaseCancelled;
    }

    void OnPurchased(PurchasableItem item)      //购买成功
    {
        Debug.Log("Purchase Succeed");
        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnPurchaseFunc);        //设置完成后执行的函数
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseSucceed"));
    }

    void OnPurchaseFailed(PurchasableItem item)     //购买失败
    {
        GlobalVars.UsingItem = PurchasedItem.None;
        Debug.Log("Purchase Failed");
        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseFailed"));
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnCancelFunc);        //设置完成后执行的函数
    }

    void OnPurchaseCancelled(PurchasableItem item)      //主动取消购买
    {
        GlobalVars.UsingItem = PurchasedItem.None;
        Debug.Log("Purchase Cancelled");
        UIWindowManager.Singleton.GetUIWindow<UIWait>().HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().ShowWindow();
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetString(Localization.instance.Get("PurchaseCancelled"));
        UIWindowManager.Singleton.GetUIWindow<UIMessageBox>().SetFunc(OnCancelFunc);        //设置完成后执行的函数
    }

    public override void OnShow()
    {
        base.OnShow();
        Unibiller.onPurchaseComplete += OnPurchased;
        Unibiller.onPurchaseFailed += OnPurchaseFailed;
        Unibiller.onPurchaseCancelled += OnPurchaseCancelled;
    }

    public override void OnHide()
    {
        base.OnHide();
        Unibiller.onPurchaseComplete -= OnPurchased;
        Unibiller.onPurchaseFailed -= OnPurchaseFailed;
        Unibiller.onPurchaseCancelled -= OnPurchaseCancelled;
        OnPurchaseFunc = null;
        OnCancelFunc = null;
    }
}
