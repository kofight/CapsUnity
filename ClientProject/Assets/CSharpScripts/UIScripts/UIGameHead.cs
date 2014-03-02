using UnityEngine;
using System.Collections;

public class UIGameHead : UIWindow
{
    GameObject m_fruitBoard;
    GameObject m_jellyBoard;
    GameObject m_scoreBoard;
    GameObject m_collectBoard;

    TweenPosition m_showCoinTweener;

    UISprite[] m_collectSprite = new UISprite[3];
    UILabel[] m_collectLabel = new UILabel[3];

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);
		
		AddChildComponentMouseClick("UseItem1Btn", delegate()
		{
            UserOrBuyItem(PurchasedItem.ItemInGame_Resort);
		});
		AddChildComponentMouseClick("UseItem2Btn", delegate()
		{
			UserOrBuyItem(PurchasedItem.ItemInGame_Hammer);
		});
        AddChildComponentMouseClick("UseItem3Btn", delegate()
        {
            if (GlobalVars.CurStageData.TimeLimit > 0 && !GameLogic.Singleton.IsStoppingTime)
            {
                UserOrBuyItem(PurchasedItem.ItemInGame_TimeStoper);
            }
            if (GlobalVars.CurStageData.ChocolateCount > 0 && !GameLogic.Singleton.IsStopingChocoGrow)
            {
                UserOrBuyItem(PurchasedItem.ItemInGame_ChocoStoper);
            }
        });

        m_fruitBoard = UIToolkits.FindChild(mUIObject.transform, "FruitBoard").gameObject;
        m_jellyBoard = UIToolkits.FindChild(mUIObject.transform, "JellyBoard").gameObject;
        m_scoreBoard = UIToolkits.FindChild(mUIObject.transform, "ScoreBoard").gameObject;
        m_collectBoard = UIToolkits.FindChild(mUIObject.transform, "CollectBoard").gameObject;

        for (int i = 0; i < 3; ++i )
        {
            m_collectSprite[i] = GetChildComponent<UISprite>("Collect" + i);
            m_collectLabel[i] = GetChildComponent<UILabel>("CollectLabel" + i);
        }

        m_showCoinTweener = mUIObject.GetComponent<TweenPosition>();
    }
    public override void OnShow()
    {
        base.OnShow();
        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            m_fruitBoard.SetActive(true);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(false);
            m_collectBoard.SetActive(false);
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
            m_collectBoard.SetActive(false);
            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "DoubleJellyBoard").gameObject.SetActive(true);
            }
            else
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "DoubleJellyBoard").gameObject.SetActive(false);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.Collect)          //处理搜集关的显示
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(false);
            m_collectBoard.SetActive(true);
            for (int i = 0; i < 3;++i )
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0)
                {
                    m_collectLabel[i].gameObject.SetActive(true);
                    if (GlobalVars.CurStageData.CollectTypes[i] <= CollectType.Collor7)
                    {
                        m_collectSprite[i].spriteName = "Item" + ((int)GlobalVars.CurStageData.CollectTypes[i] - (int)CollectType.Collor1 + 1).ToString();
                    }
                    else
                    {
                        m_collectSprite[i].spriteName = GlobalVars.CurStageData.CollectTypes[i].ToString();
                    }
                }
                else
                {
                    m_collectLabel[i].gameObject.SetActive(false);
                }
            }
        }
        if (GlobalVars.CurStageData.Target == GameTarget.GetScore)
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(true);
            m_collectBoard.SetActive(false);
            UIToolkits.FindComponent<NumberDrawer>(m_scoreBoard.transform, "ScoreNum").SetNumber(GlobalVars.CurStageData.StarScore[0]);
        }

        UISprite item3Icon = GetChildComponent<UISprite>("Item3Icon");

        if (GlobalVars.CurStageData.TimeLimit > 0)
        {
            item3Icon.spriteName = PurchasedItem.ItemInGame_TimeStoper.ToString();
            item3Icon.transform.parent.gameObject.SetActive(true);
        }
        else if (GlobalVars.CurStageData.ChocolateCount > 0)
        {
            item3Icon.spriteName = PurchasedItem.ItemInGame_ChocoStoper.ToString();
            item3Icon.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            item3Icon.transform.parent.gameObject.SetActive(false);
        }

        RefreshTarget();
    }

    public void ShowCoin(bool bShow)
    {
        m_showCoinTweener.Play(bShow);
        UILabel coinLabel = GetChildComponent<UILabel>("CoinCount");
        coinLabel.text = ((int)Unibiller.GetCurrencyBalance("gold")).ToString();
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
            if (GlobalVars.CurStageData.GetJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "JellyNum").SetNumber(GameLogic.Singleton.PlayingStageData.GetSingleJellyCount());
            }

            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "DoubleJellyNum").SetNumber(GameLogic.Singleton.PlayingStageData.GetDoubleJellyCount());
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.Collect)          //处理搜集关的显示
        {
            for (int i = 0; i < 3; ++i)
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0)
                {
                    m_collectLabel[i].text = GameLogic.Singleton.PlayingStageData.CollectCount[i].ToString() + "/" + GlobalVars.CurStageData.CollectCount[i].ToString();
                }
            }
        }
	}
	
	void UserOrBuyItem(PurchasedItem item)
	{
        if (GameLogic.Singleton.GetGameFlow() != TGameFlow.EGameState_Playing || !GameLogic.Singleton.IsMoveAble())
        {
            return;
        }

        GlobalVars.UsingItem = item;

        if (Unibiller.GetCurrencyBalance("gold") < CapsConfig.GetItemPrice(item))       //若钱不够，购买窗口
        {
            UIPurchaseNotEnoughMoney uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
            uiWindow.ShowWindow();
            uiWindow.OnPurchaseFunc = delegate()
            {
                UserOrBuyItem(item);
            };
            uiWindow.OnCancelFunc = null;                                               //取消时什么都不做，直接回到游戏
            return;
        }
		
        //先判断是否够钱
        if (item == PurchasedItem.ItemInGame_Hammer)
        {
            UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseTarget>();
            uiWindow.ShowWindow();
        }
        else
        {
            UIWindow uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNoTarget>();
            uiWindow.ShowWindow();
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
