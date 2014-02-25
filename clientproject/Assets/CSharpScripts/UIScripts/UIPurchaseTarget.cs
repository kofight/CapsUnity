using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseTarget : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
	public delegate void OnPurchaseFunc();
    GameObject m_target;
	
	public OnPurchaseFunc OnPurchase; 
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
        m_target = GameObject.Find("TargetBlock");
        m_target.SetActive(false);
    }
    public override void OnShow()
    {
        base.OnShow();
        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(true);

        for (int i = 0; i < GameLogic.BlockCountX; ++i)
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j)
            {
                CapBlock pBlock = GameLogic.Singleton.GetBlock(new Position(i, j));
                if (pBlock != null)
                {
                    GlobalVars.UsingItemTarget = new Position(i, j);
                    SetTarget(GlobalVars.UsingItemTarget);
                    break;
                }
            }
        }

        Debug.Log(GlobalVars.UsingItem.ToString());

        m_msgLabel.text = Localization.instance.Get("Use_" + GlobalVars.UsingItem.ToString());
        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString();
    }

    public override void OnHide()
    {
        base.OnHide();
        m_target.SetActive(false);
    }

    public override void OnHideEffectPlayOver()
    {
        base.OnHideEffectPlayOver();
        UIGameHead gamehead = UIWindowManager.Singleton.GetUIWindow<UIGameHead>();
        gamehead.ShowCoin(false);
    }

    public void SetTarget(Position pos)     //设置目标点
    {
        if (!pos.IsAvailable())
        {
            m_target.SetActive(false);
            return;
        }
        m_target.SetActive(true);
        m_target.transform.localPosition = new Vector3(GameLogic.Singleton.GetXPos(pos.x), -GameLogic.Singleton.GetYPos(pos.x, pos.y));
    }

    public void SetString(string str)
    {
        m_msgLabel.text = str;
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            if (GlobalVars.UsingItem == PurchasedItem.ItemInGame_Hammer)
            {
                if (Unibiller.DebitBalance("gold", CapsConfig.GetItemPrice(GlobalVars.UsingItem)))
                {
                    GA.API.Business.NewEvent("BuyHammer", "Coins", CapsConfig.GetItemPrice(GlobalVars.UsingItem));
                    GameLogic.Singleton.ClearHelpPoint();
                    GameLogic.Singleton.EatBlock(GlobalVars.UsingItemTarget, CapsConfig.EatEffect);                  //使用锤子
                }
                m_target.SetActive(false);
            }
        });
    }

    public void OnCancelClicked()
    {
        HideWindow(delegate()
        {
            
        });
        if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
        {
            UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
        }
    }
}
