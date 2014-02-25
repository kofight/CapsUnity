using UnityEngine;
using System.Collections;

public class UIStageInfo : UIWindow 
{
    UILabel m_levelLabel;

    UILabel[] m_itemCostLabels = new UILabel[3];
    UIToggle [] m_itemToggles = new UIToggle [3];
    PurchasedItem [] m_items = new PurchasedItem [3];

    int m_moneyCost = 0;

    public int GetCurCost() { return m_moneyCost; }

    public override void OnCreate()
    {
        base.OnCreate();

        m_levelLabel = GetChildComponent<UILabel>("LevelLabel");

        for (int i = 0; i < 3; ++i )
        {
            m_itemCostLabels[i] = GetChildComponent<UILabel>("Item" + (i+1).ToString() +"Cost");
            m_itemToggles[i] = GetChildComponent<UIToggle>("Item" + (i + 1).ToString() + "Btn");
			m_itemToggles[i].SetWithoutTrigger(false);
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
                    ShowWindow();
                };

                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().OnPurchaseFunc = delegate()
                {
                    ShowWindow();
                };

                GlobalVars.UsingItem = m_items[curItemIndex];

                UIWindowManager.Singleton.GetUIWindow<UIPurchaseNotEnoughMoney>().ShowWindow();
            });
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
        }

        NumberDrawer number = GetChildComponent<NumberDrawer>("StageTarget");
        number.SetNumber(GlobalVars.CurStageData.StarScore[2]);

        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().HideWindow();
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
