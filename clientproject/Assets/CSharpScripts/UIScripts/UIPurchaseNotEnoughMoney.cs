using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNotEnoughMoney : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
    UISprite m_itemIcon;
    UISprite m_buyItemBtnSprite;
    GameObject m_buy6Board;
    GameObject m_buy12Board;

    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("GetCoinBtn", delegate()
		{
			HideWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().ShowWindow();
            //UIWindowManager.Singleton.GetUIWindow<UIWait>().SetString(Localization.instance.Get("WaitForPurchase"));

            int coinNeeded = CapsConfig.GetItemPrice(GlobalVars.UsingItem) - (int)Unibiller.GetCurrencyBalance("gold");
            if (coinNeeded <= 60)
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.60coins");
                GlobalVars.PurchasingItemName = "60Coins";
                GlobalVars.PurchasingItemPrice = 99;
            }
            else
            {
                Unibiller.initiatePurchase("com.linkrstudio.jellycraft.130coins");
                GlobalVars.PurchasingItemName = "130Coins";
                GlobalVars.PurchasingItemPrice = 199;
            }
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
        m_buy6Board = UIToolkits.FindChild(mUIObject.transform, "Price6").gameObject;
        m_buy12Board = UIToolkits.FindChild(mUIObject.transform, "Price12").gameObject;
    }

    public void OnCloseBtn()
    {
        HideWindow(delegate()
        {
			if (GlobalVars.OnCancelFunc != null)
            {
				GlobalVars.OnCancelFunc();
            }
            if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
            {
                GameLogic.Singleton.ResumeGame();
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
            m_buy6Board.SetActive(true);
            m_buy12Board.SetActive(false);
        }
        else
        {
            m_buy6Board.SetActive(false);
            m_buy12Board.SetActive(true);
        }

        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            GameLogic.Singleton.PauseGame();
        }
    }
}
