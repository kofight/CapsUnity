using UnityEngine;
using System.Collections;
using System;

public class UIStore : UIWindow 
{
    public override void OnCreate()
    {
        base.OnCreate();
        //AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        //AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow();
        });

        AddChildComponentMouseClick("BuyItem1Btn", delegate()
        {
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.140coins");
        });
        AddChildComponentMouseClick("BuyItem2Btn", delegate()
        {
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.800coins");
        });
        AddChildComponentMouseClick("BuyItem3Btn", delegate()
        {
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.1700coins");
        });
        AddChildComponentMouseClick("BuyItem4Btn", delegate()
        {
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.4500coins");
        });
        AddChildComponentMouseClick("BuyItem5Btn", delegate()
        {
            Unibiller.initiatePurchase("com.linkrstudio.jellycraft.9500coins");
        });

        Unibiller.onPurchaseComplete += OnPurchased;
        Unibiller.onPurchaseFailed += OnPurchaseFailed;
        Unibiller.onPurchaseCancelled += OnPurchaseCancelled;
    }

    void OnPurchased(PurchasableItem item)      //购买成功
    {
        Debug.Log("Purchase Succeed");
        HideWindow();
    }

    void OnPurchaseFailed(PurchasableItem item)     //购买失败
    {
        Debug.Log("Purchase Failed");
        HideWindow();
    }

    void OnPurchaseCancelled(PurchasableItem item)      //主动取消购买
    {
        Debug.Log("Purchase Cancelled");
        HideWindow();
    }

    public override void OnShow()
    {
        base.OnShow();
    }
}
