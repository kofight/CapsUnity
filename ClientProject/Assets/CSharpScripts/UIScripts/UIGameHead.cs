using UnityEngine;
using System.Collections;

public class UIGameHead : UIWindow
{
    UILabel [] m_itemUILabel = new UILabel [2];

    GameObject m_fruitBoard;
    GameObject m_jellyBoard;
    GameObject m_scoreBoard;

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);
		
		AddChildComponentMouseClick("UseItem1Btn", delegate()
		{
			UserOrBuyItem(PurchasedItem.Item_Hammer);
		});
		AddChildComponentMouseClick("UseItem2Btn", delegate()
		{
			UserOrBuyItem(PurchasedItem.Item_PlusStep);
		});

        m_fruitBoard = UIToolkits.FindChild(mUIObject.transform, "FruitBoard").gameObject;
        m_jellyBoard = UIToolkits.FindChild(mUIObject.transform, "JellyBoard").gameObject;
        m_scoreBoard = UIToolkits.FindChild(mUIObject.transform, "ScoreBoard").gameObject;

        for (int i = 0; i < 2; ++i )
        {
            m_itemUILabel[i] = GetChildComponent<UILabel>("ItemCount" + (i + 1));
        }
    }
    public override void OnShow()
    {
        base.OnShow();
        RefreshItemCount();
        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            m_fruitBoard.SetActive(true);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(false);
            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                UIToolkits.FindChild(m_fruitBoard.transform, "Fruit1Board").gameObject.SetActive(true);
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumTotal1").spriteName = GlobalVars.CurStageData.Nut1Count.ToString();
            }
            else
            {
                UIToolkits.FindChild(m_fruitBoard.transform, "Fruit1Board").gameObject.SetActive(false);
            }
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                UIToolkits.FindChild(m_fruitBoard.transform, "Fruit2Board").gameObject.SetActive(true);
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumTotal2").spriteName = GlobalVars.CurStageData.Nut2Count.ToString();
            }
            else
            {
                UIToolkits.FindChild(m_fruitBoard.transform, "Fruit2Board").gameObject.SetActive(false);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(true);
            m_scoreBoard.SetActive(false);
            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "DoubleJellyBoard").gameObject.SetActive(true);
            }
            else
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "DoubleJellyBoard").gameObject.SetActive(false);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(true);
            UIToolkits.FindComponent<NumberDrawer>(m_scoreBoard.transform, "ScoreNum").SetNumber(GlobalVars.CurStageData.StarScore[0]);
        }

        RefreshTarget();
    }
	
	public void RefreshTarget()
	{
        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumCur1").spriteName = GameLogic.Singleton.PlayingStageData.Nut1Count.ToString();
            }
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumCur2").spriteName = GameLogic.Singleton.PlayingStageData.Nut2Count.ToString();
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(true);
            m_scoreBoard.SetActive(false);
            if (GlobalVars.CurStageData.GetJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "JellyNum").SetNumber(GameLogic.Singleton.PlayingStageData.GetSingleJellyCount());
            }

            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "DoubleJellyNum").SetNumber(GameLogic.Singleton.PlayingStageData.GetDoubleJellyCount());
            }
        }
	}
	
	void UserOrBuyItem(PurchasedItem item)
	{
        if (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_Playing)
        {
            return;
        }

        GlobalVars.UsingItem = item;

        //先判断是否够钱
        if (item == PurchasedItem.Item_Hammer)
        {
            if (Unibiller.GetCurrencyBalance("gold") < 6)       //若钱不够，购买窗口
            {
                UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
                uiWindow.ShowWindow();
            }
            else //若钱够，弹使用道具窗口
            {
                UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>();
                uiWindow.ShowWindow();
            }
        }

        if (item == PurchasedItem.Item_PlusStep)
        {
            if (Unibiller.GetCurrencyBalance("gold") < 6)       //若钱不够，购买窗口
            {
                UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
                uiWindow.ShowWindow();
            }
            else //若钱够，弹使用道具窗口
            {
                UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>();
                uiWindow.ShowWindow();
            }
        }

        ////若没有存货
        //if (GlobalVars.PurchasedItemArray[(int)item] == 0)
        //{
        //    UIPurchaseNoTarget purchaseWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>();
        //    purchaseWindow.ShowWindow();
        //    //purchaseWindow.SetString("Are you sure about using the item?");
        //    purchaseWindow.OnPurchase = delegate()
        //    {
        //        if (item == PurchasedItem.Item_PlusStep)
        //        {
        //            if (GlobalVars.Coins > 0)
        //            {
        //                --GlobalVars.Coins;
        //                GA.API.Business.NewEvent("BuyStep", "RMB", 1);
        //                PlayerPrefs.SetInt("Coins", GlobalVars.Coins);
        //                if(GlobalVars.CurStageData.StepLimit > 0)
        //                    GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
        //                else if(GlobalVars.CurStageData.TimeLimit > 0)
        //                    GameLogic.Singleton.AddGameTime(15);        //Add 15 Seconds
        //            }
        //        }
        //        else if (item == PurchasedItem.Item_Hammer)
        //        {
        //            GameLogic.Singleton.UsingItem = item;                       //进入使用道具状态，等着选目标
        //        }
        //    };
        //    return;
        //}

        //UIUseItem useItemWindow = UIWindowManager.Singleton.GetUIWindow<UIUseItem>();
        //useItemWindow.ShowWindow();
        //useItemWindow.OnUse = delegate()
        //{
        //    if (item == PurchasedItem.Item_Hammer)
        //    {
        //        GameLogic.Singleton.UsingItem = item;
        //    }
        //    else if (item == PurchasedItem.Item_PlusStep)
        //    {
        //        GameLogic.Singleton.PlayingStageData.StepLimit += 5;        //步数加5
        //        --GlobalVars.PurchasedItemArray[(int)item];                     //减少道具数量
        //        RefreshItemCount();
        //        PlayerPrefsExtend.SetIntArray("PurchasedItemArray", GlobalVars.PurchasedItemArray);
        //    }
        //};
	}

    public void RefreshItemCount()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (GlobalVars.PurchasedItemArray[i] == 0)
            {
                m_itemUILabel[i].gameObject.SetActive(false);
            }
            else
            {
                m_itemUILabel[i].gameObject.SetActive(true);
                m_itemUILabel[i].text = GlobalVars.PurchasedItemArray[i].ToString();
            }
        }
    }

    private void OnEditStageClicked()
    {
        if (UIWindowManager.Singleton.GetUIWindow<UIStageEditor>() == null)
        {
            UIWindowManager.Singleton.CreateWindow<UIStageEditor>(UIWindowManager.Anchor.Right);
        }
        UIWindowManager.Singleton.GetUIWindow<UIStageEditor>().ShowWindow();        //显示编辑窗口
        GlobalVars.EditStageMode = true;        //编辑器里自动进入关卡编辑模式
    }
}
