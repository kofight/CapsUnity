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
    public UILabel[] m_collectLabel = new UILabel[3];
    UISprite[] m_coolDownSprite = new UISprite[3];
    UIButton[] m_itemBtn = new UIButton[3];
    PurchasedItem[] m_items = new PurchasedItem[3];

    UISprite[] m_background = new UISprite[3];

    UIButton m_debugBtn;

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("EditorBtn", OnEditStageClicked);

        m_fruitBoard = UIToolkits.FindChild(mUIObject.transform, "FruitBoard").gameObject;
        m_jellyBoard = UIToolkits.FindChild(mUIObject.transform, "JellyBoard").gameObject;
        m_scoreBoard = UIToolkits.FindChild(mUIObject.transform, "ScoreBoard").gameObject;
        m_collectBoard = UIToolkits.FindChild(mUIObject.transform, "CollectBoard").gameObject;

        for (int i = 0; i < 3; ++i )
        {
            m_collectSprite[i] = GetChildComponent<UISprite>("Collect" + i);
            m_collectLabel[i] = GetChildComponent<UILabel>("CollectLabel" + i);
            m_coolDownSprite[i] = GetChildComponent<UISprite>("CoolDown" + (i + 1).ToString());
			
			m_coolDownSprite[i].gameObject.SetActive(false);
			
            m_itemBtn[i] = GetChildComponent<UIButton>("UseItem" + (i+1).ToString() + "Btn");

            m_background[i] = GetChildComponent<UISprite>("Background" + (i + 1).ToString());
        }
		
		UIToolkits.AddChildComponentMouseClick(m_itemBtn[0].gameObject, delegate()
        {
			UserOrBuyItem(PurchasedItem.ItemInGame_Resort);
        });
		UIToolkits.AddChildComponentMouseClick(m_itemBtn[1].gameObject, delegate()
        {
			UserOrBuyItem(PurchasedItem.ItemInGame_Hammer);
        });
		UIToolkits.AddChildComponentMouseClick(m_itemBtn[2].gameObject, delegate()
        {
			if (GlobalVars.CurStageData.TimeLimit > 0 && !GameLogic.Singleton.IsStoppingTime)
            {
				UserOrBuyItem(PurchasedItem.ItemInGame_TimeStoper);
            }
            else if (GlobalVars.CurStageData.ChocolateCount > 0 && !GameLogic.Singleton.IsStopingChocoGrow)
            {
				UserOrBuyItem(PurchasedItem.ItemInGame_ChocoStoper);
            }
        });

        m_showCoinTweener = mUIObject.GetComponent<TweenPosition>();
        m_debugBtn = GetChildComponent<UIButton>("EditorBtn");
    }

    public override void OnShow()
    {
        base.OnShow();
		
		m_items[0] = PurchasedItem.ItemInGame_Resort;
        m_items[1] = PurchasedItem.ItemInGame_Hammer;
        if (GlobalVars.CurStageData.TimeLimit > 0)
        {
            m_items[2] = PurchasedItem.ItemInGame_TimeStoper;
        }
        else if (GlobalVars.CurStageData.ChocolateCount > 0)
        {
            m_items[2] = PurchasedItem.ItemInGame_ChocoStoper;
        }
		
        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            m_fruitBoard.SetActive(true);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(false);
            m_collectBoard.SetActive(false);

            Transform fruit1Trans = UIToolkits.FindChild(m_fruitBoard.transform, "Fruit1Board");
            Transform fruit2Trans = UIToolkits.FindChild(m_fruitBoard.transform, "Fruit2Board");
            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                fruit1Trans.gameObject.SetActive(true);
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumTotal1").spriteName = "BaseNum" + GlobalVars.CurStageData.Nut1Count.ToString();
            }
            else
            {
                fruit1Trans.gameObject.SetActive(false);
            }
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                fruit2Trans.gameObject.SetActive(true);
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumTotal2").spriteName = "BaseNum" + GlobalVars.CurStageData.Nut2Count.ToString();
            }
            else
            {
                fruit2Trans.gameObject.SetActive(false);
            }
            //居中对齐
            if (GlobalVars.CurStageData.Nut1Count > 0 && GlobalVars.CurStageData.Nut2Count > 0)     //若两个水果都有
            {
                fruit1Trans.LocalPositionX(10);
                fruit2Trans.LocalPositionX(201);
            }
            else if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                fruit1Trans.LocalPositionX(111);
            }
            else if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                fruit2Trans.LocalPositionX(111);
            }

            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                GameLogic.Singleton.CollectTargetUIPos[0] = new Vector3(-59.63453f + CapsApplication.Singleton.Width / 2 + fruit1Trans.localPosition.x, 1.562183f - 38.568f, 0);
            }
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                GameLogic.Singleton.CollectTargetUIPos[1] = new Vector3(-59.63453f + CapsApplication.Singleton.Width / 2 + fruit2Trans.localPosition.x, 1.562183f - 38.5681f, 0);
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

            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "JellyBoard").LocalPositionX(27);
                UIToolkits.FindChild(m_jellyBoard.transform, "DoubleJellyBoard").LocalPositionX(187);
            }
            else
            {
                UIToolkits.FindChild(m_jellyBoard.transform, "JellyBoard").LocalPositionX(123);
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.Collect)          //处理搜集关的显示
        {
            m_fruitBoard.SetActive(false);
            m_jellyBoard.SetActive(false);
            m_scoreBoard.SetActive(false);
            m_collectBoard.SetActive(true);
            int collectTypeCount = 0;
            for (int i = 0; i < 3;++i )
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0)
                {
                    m_collectLabel[i].gameObject.SetActive(true);
                    ++collectTypeCount;

                    switch (GlobalVars.CurStageData.CollectSpecial[i])
                    {
                        case TSpecialBlock.ESpecial_Normal:
                            {
                                m_collectSprite[i].spriteName = "Item" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_NormalPlus6:
                            {
                                m_collectSprite[i].spriteName = "TimeAdded" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            }
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir0:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_3";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir1:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_1";
                            break;
                        case TSpecialBlock.ESpecial_EatLineDir2:
                            m_collectSprite[i].spriteName = "Line" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None) + "_2";
                            break;
                        case TSpecialBlock.ESpecial_Bomb:
                            m_collectSprite[i].spriteName = "Bomb" + (int)(GlobalVars.CurStageData.CollectColors[i] - TBlockColor.EColor_None);
                            break;
                        case TSpecialBlock.ESpecial_EatAColor:
                            m_collectSprite[i].spriteName = "Rainbow";
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    m_collectLabel[i].gameObject.SetActive(false);
                }
            }

            if (collectTypeCount == 3)
            {
                int interval = 130;
                for (int i = 0; i < 3; ++i )
                {
                    m_collectLabel[i].transform.LocalPositionX(i * interval);
                }
            }
            else if (collectTypeCount == 2)
            {
                int interval = 150;
                int curPosX = 30;
                for (int i = 0; i < 3; ++i)
                {
                    if (GlobalVars.CurStageData.CollectCount[i] > 0)
                    {
                        m_collectLabel[i].transform.LocalPositionX(curPosX);
                        curPosX += interval;
                    }
                }
            }
            else if (collectTypeCount == 1)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (GlobalVars.CurStageData.CollectCount[i] > 0)
                    {
                        m_collectLabel[i].transform.LocalPositionX(144);
                        break;
                    }
                }
            }

            GameLogic.Singleton.CollectTargetUIPos[0] = new Vector3(-59.63453f + CapsApplication.Singleton.Width / 2 + m_collectLabel[0].transform.localPosition.x, 1.562183f - 38.568f, 0);
            GameLogic.Singleton.CollectTargetUIPos[1] = new Vector3(-59.63453f + CapsApplication.Singleton.Width / 2 + m_collectLabel[1].transform.localPosition.x, 1.562183f - 38.5681f, 0);
            GameLogic.Singleton.CollectTargetUIPos[2] = new Vector3(-59.63453f + CapsApplication.Singleton.Width / 2 + m_collectLabel[2].transform.localPosition.x, 1.562183f - 38.5681f, 0);
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

        for (int i = 0; i < 3; ++i)
        {
            if (CapsConfig.ItemUnLockLevelArray[(int)m_items[i]] <= GlobalVars.AvailabeStageCount || GlobalVars.DeveloperMode)
            {
                if (i == 2)
                {
                    if (GlobalVars.CurStageData.TimeLimit > 0)
                    {
                        item3Icon.spriteName = PurchasedItem.ItemInGame_TimeStoper.ToString();
                        m_itemBtn[2].gameObject.SetActive(true);
                    }
                    else if (GlobalVars.CurStageData.ChocolateCount > 0)
                    {
                        item3Icon.spriteName = PurchasedItem.ItemInGame_ChocoStoper.ToString();
                        m_itemBtn[2].gameObject.SetActive(true);
                    }
                    else
                    {
                        m_itemBtn[2].gameObject.SetActive(false);
                    }
                }
                else
                {
                    m_itemBtn[i].gameObject.SetActive(true);
                }
            }
            else
            {
                m_itemBtn[i].gameObject.SetActive(false);
            }
        }


        UpdateItemButtons();

        RefreshTarget();

        if (GlobalVars.DeveloperMode)
        {
            m_debugBtn.gameObject.SetActive(true);
        }
        else
        {
            m_debugBtn.gameObject.SetActive(false);
        }
    }

    public void UpdateCoolDown(float val)       //更新coolDown的值
    {
        m_coolDownSprite[2].fillAmount = val;
    }

    public void UpdateItemButtons()
    {
        for (int i = 0; i < 3; ++i)
        {
            if (m_itemBtn[i].gameObject.activeSelf)       //判断道具是否已经解锁?
            {
                if (i == 2 && (GameLogic.Singleton.IsStoppingTime || GameLogic.Singleton.IsStopingChocoGrow))       //临时锁定状态
                {
                    m_itemBtn[i].enabled = false;
                    m_coolDownSprite[i].gameObject.SetActive(true);
                }
                else
                {
                    m_coolDownSprite[i].gameObject.SetActive(false);
                    m_itemBtn[i].enabled = true;
                }
            }
        }
    }

    public void ShowCoin(bool bShow)
    {
        if (bShow)
        {
            m_showCoinTweener.delay = 0;
        }
        else
        {
            m_showCoinTweener.delay = 1;
        }
        m_showCoinTweener.Play(bShow);
        NumberDrawer coinLabel = GetChildComponent<NumberDrawer>("CoinCount");
        coinLabel.SetNumber((int)Unibiller.GetCurrencyBalance("gold"));
    }
	
	public void RefreshTarget()
	{
        if (GlobalVars.CurStageData.Target == GameTarget.BringFruitDown)
        {
            if (GlobalVars.CurStageData.Nut1Count > 0)
            {
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumCur1").spriteName = "BaseNum" + GameLogic.Singleton.PlayingStageData.Nut1Count.ToString();
            }
            if (GlobalVars.CurStageData.Nut2Count > 0)
            {
                UIToolkits.FindComponent<UISprite>(m_fruitBoard.transform, "FruitNumCur2").spriteName = "BaseNum" + GameLogic.Singleton.PlayingStageData.Nut2Count.ToString();
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            if (GlobalVars.CurStageData.GetJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "JellyNum").SetNumberRapid(GameLogic.Singleton.PlayingStageData.GetSingleJellyCount());
            }

            if (GlobalVars.CurStageData.GetDoubleJellyCount() > 0)
            {
                UIToolkits.FindComponent<NumberDrawer>(m_jellyBoard.transform, "DoubleJellyNum").SetNumberRapid(GameLogic.Singleton.PlayingStageData.GetDoubleJellyCount());
            }
        }
        else if (GlobalVars.CurStageData.Target == GameTarget.Collect)          //处理搜集关的显示
        {
            for (int i = 0; i < 3; ++i)
            {
                if (GlobalVars.CurStageData.CollectCount[i] > 0)
                {
                    m_collectLabel[i].text = GameLogic.Singleton.PlayingStageData.CollectCount[i].ToString() + "/" + GlobalVars.CurStageData.CollectCount[i].ToString();
                    if (GameLogic.Singleton.PlayingStageData.CollectCount[i] >= GlobalVars.CurStageData.CollectCount[i])
                    {
                        m_collectLabel[i].color = new Color(0, 1, 0);
                    }
                    else
                    {
                        m_collectLabel[i].color = new Color(0, 0, 0);
                    }
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

        if (GlobalVars.UsingItem != PurchasedItem.None)
        {
            return;
        }

        GlobalVars.UsingItem = item;

        //先判断是否够钱
        if (Unibiller.GetCurrencyBalance("gold") < CapsConfig.GetItemPrice(item))       //若钱不够，购买窗口
        {
            UIPurchaseNotEnoughMoney uiWindow = UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>();
            uiWindow.ShowWindow();
            uiWindow.OnPurchaseFunc = delegate()
            {
                UserOrBuyItem(item);
            };
            uiWindow.OnCancelFunc = delegate()
            {
                GlobalVars.UsingItem = PurchasedItem.None;
            };
            return;
        }
        
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
