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
        if (GlobalVars.UsingItem == PurchasedItem.Item_Hammer)
        {
            m_msgLabel.text = Localization.instance.Get("Use_Hammer");
            m_costLabel.text = "50";
        }
    }

    public override void OnHide()
    {
        base.OnHide();
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
            if (GlobalVars.UsingItem == PurchasedItem.Item_Hammer)
            {
                if (Unibiller.DebitBalance("gold", 50))
                {
                    GA.API.Business.NewEvent("BuyHammer", "Coins", 50);
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
