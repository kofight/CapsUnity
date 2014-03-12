using UnityEngine;
using System.Collections;

public class UIStageInfo : UIWindow 
{
    UILabel m_levelLabel;
    UILabel m_totalCostLabel;

    UILabel[] m_itemCostLabels = new UILabel[3];
    UIToggle [] m_itemToggles = new UIToggle [3];
    PurchasedItem [] m_items = new PurchasedItem [3];
    UISprite[] m_lockItemSprite = new UISprite[3];
    UISprite[] m_background = new UISprite[3];
    UISprite[] m_backgroundFrame = new UISprite[3];

    int m_moneyCost = 0;

    public int GetCurCost() { return m_moneyCost; }

    public override void OnCreate()
    {
        base.OnCreate();

        m_levelLabel = GetChildComponent<UILabel>("LevelLabel");
        m_totalCostLabel = GetChildComponent<UILabel>("ItemTotalCost");

        for (int i = 0; i < 3; ++i )
        {
            m_itemCostLabels[i] = GetChildComponent<UILabel>("Item" + (i+1).ToString() +"Cost");
            m_itemToggles[i] = GetChildComponent<UIToggle>("Item" + (i + 1).ToString() + "Btn");
			m_itemToggles[i].SetWithoutTrigger(false);
            m_lockItemSprite[i] = GetChildComponent<UISprite>("LockItem" + (i + 1).ToString());

            m_background[i] = GetChildComponent<UISprite>("Background" + (i + 1).ToString());
            m_backgroundFrame[i] = GetChildComponent<UISprite>("BackgroundFrame" + (i + 1).ToString());

            EventDelegate.Set(m_itemToggles[i].onChange, OnToggle);
        }

        AddChildComponentMouseClick("CloseBtn", OnCloseClicked);
        AddChildComponentMouseClick("PlayBtn", OnPlayClicked);
    }

    public void OnToggle()
    {
        m_moneyCost = 0;
        int curItemIndex = -1;
        for (int i = 0; i < 3; ++i)
        {
            if (UIToggle.current == m_itemToggles[i])
            {
                curItemIndex = i;
            }
            if (m_itemToggles[i].value)
            {
                GlobalVars.StartStageItem[i] = m_items[i];
                m_moneyCost += CapsConfig.GetItemPrice(m_items[i]);
            }
            else
            {
                GlobalVars.StartStageItem[i] = PurchasedItem.None;
            }
        }

        if (m_moneyCost > Unibiller.GetCurrencyBalance("gold"))
        {
			UIToggle.current.SetWithoutTrigger(false);     //把值置回来
            m_moneyCost -= CapsConfig.GetItemPrice(m_items[curItemIndex]);
            GlobalVars.StartStageItem[curItemIndex] = PurchasedItem.None;
			
            HideWindow(delegate()
            {
                //弹出购买金币提示
                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().OnCancelFunc = delegate()
                {
                    GlobalVars.UsingItem = PurchasedItem.None;
                    ShowWindow();
                };

                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().OnPurchaseFunc = delegate()
                {
                    GlobalVars.UsingItem = PurchasedItem.None;
                    ShowWindow();
                };

                GlobalVars.UsingItem = m_items[curItemIndex];

                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().ShowWindow();
            });
        }
        else
        {
            if (UIToggle.current.value)
            {
                NGUITools.PlaySound(CapsConfig.CurAudioList.PurchaseClip);
            }
        }
        RefreshTotalMoney();
    }

    void RefreshTotalMoney()
    {
        if (m_moneyCost == 0)
        {
            m_totalCostLabel.gameObject.SetActive(false);
        }
        else
        {
            m_totalCostLabel.gameObject.SetActive(true);
            m_totalCostLabel.text = m_moneyCost.ToString();
        }
    }

    public override void OnShow()
    {
        base.OnShow();

        m_levelLabel.text = System.String.Format(Localization.instance.Get("LevelName"), GlobalVars.CurStageNum);

        for (int i = 0; i < 3; ++i )
        {
            UISprite star = GetChildComponent<UISprite>("Star" + (i + 1));
            if (GlobalVars.StageStarArray[GlobalVars.CurStageNum - 1] > i)
            {
                star.spriteName = "Star_Large";
            }
            else
            {
				star.spriteName = "Grey_Star_Large";
            }
        }

        UISprite itemIcon = GetChildComponent<UISprite>("Item1Icon");
        if (GlobalVars.CurStageData.StepLimit > 0)
        {
            m_items[0] = PurchasedItem.ItemPreGame_PlusStep;
        }
        else if(GlobalVars.CurStageData.TimeLimit > 0)
        {
            m_items[0] = PurchasedItem.ItemPreGame_PlusTime;
        }

        m_items[1] = PurchasedItem.ItemPreGame_AddEatColor;
        m_items[2] = PurchasedItem.ItemPreGame_ExtraScore;

        itemIcon.spriteName = m_items[0].ToString();

        for (int i = 0; i < 3; ++i )
        {
            m_itemCostLabels[i].text = CapsConfig.GetItemPrice(m_items[i]).ToString();
            m_itemToggles[i].SetWithoutTrigger(false);

            if (CapsConfig.ItemUnLockLevelArray[(int)m_items[i]] <= GlobalVars.AvailabeStageCount || GlobalVars.DeveloperMode)       //判断道具是否已经解锁?
            {
                m_lockItemSprite[i].gameObject.SetActive(false);
                m_itemToggles[i].enabled = true;
                m_itemCostLabels[i].gameObject.SetActive(true);
                m_background[i].spriteName = "Item_Large";
                m_backgroundFrame[i].spriteName = "ItemBoard_Large";
            }
            else
            {
				m_lockItemSprite[i].gameObject.SetActive(true);
                m_itemToggles[i].enabled = false;
                m_itemCostLabels[i].gameObject.SetActive(false);
                m_background[i].spriteName = "LockItemBtn";
                m_backgroundFrame[i].spriteName = "LockedItemBoard";
            }
        }

        NumberDrawer number = GetChildComponent<NumberDrawer>("StageTarget");
        number.SetNumber(GlobalVars.CurStageData.StarScore[2]);

        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();

        RefreshTotalMoney();
    }

    public void OnCloseClicked()
    {
        HideWindow();

        if (CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
        {
            CapsApplication.Singleton.ChangeState((int)StateEnum.Login);        //返回地图界面
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
            UIWindowManager.Singleton.GetUIWindow<UIMap>().ShowWindow();        //返回地图，不需要刷新按钮
            LoginState.Instance.CurFlow = TLoginFlow.LoginFlow_Map;         //切换流程到显示地图
        }
        else
        {
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        }

        ClearToggles();
    }

    public void ClearToggles()
    {
        //清理CheckBox
        for (int i = 0; i < 3; ++i)
        {
            if (m_itemToggles[i].value)
            {
                m_itemToggles[i].SetWithoutTrigger(false);
            }
        }
        m_moneyCost = 0;
    }

    private void OnPlayClicked()
    {
        if (Unibiller.DebitBalance("gold", m_moneyCost))        //消费
        {
            //使用道具
            for (int i = 0; i < 3; ++i)
            {
                if (m_itemToggles[i].value)
                {
                    GlobalVars.StartStageItem[i] = m_items[i];
                }
                else
                {
                    GlobalVars.StartStageItem[i] = PurchasedItem.None;
                }
            }
            ClearToggles();
        }
        else        //若钱不够
        {
            return;
        }

        GlobalVars.UseHeart();      //使用一颗心

        if (CapsApplication.Singleton.CurStateEnum != StateEnum.Game)
        {
            UIWindowManager.Singleton.GetUIWindow<UIMap>().HideWindow();
            HideWindow();

            
            UIWindowManager.Singleton.GetUIWindow("UILoading").ShowWindow(
            delegate()
            {
                CapsApplication.Singleton.ChangeState((int)StateEnum.Game);
            }
            );

            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
        }
        else
        {
            HideWindow(delegate()
            {
                UIWindowManager.Singleton.GetUIWindow<UIDialog>().TriggerDialog(GlobalVars.CurStageNum, DialogTriggerPos.StageStart, delegate()
                {
                    GameLogic.Singleton.Init();
                    GameLogic.Singleton.StartGame();
                    UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
                    UIWindowManager.Singleton.GetUIWindow<UIGameHead>().ShowWindow();
                    UIWindowManager.Singleton.GetUIWindow<UIGameBottom>().ShowWindow();
                });
            });
        }
    }
}
