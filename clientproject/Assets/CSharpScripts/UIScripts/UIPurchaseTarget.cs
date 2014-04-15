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

        m_msgLabel.text = Localization.instance.Get("Use_" + GlobalVars.UsingItem.ToString());
        m_costLabel.text = CapsConfig.GetItemPrice(GlobalVars.UsingItem).ToString();

        //若冰块关，找个冰块
        if (GlobalVars.CurStageData.Target == GameTarget.ClearJelly)
        {
            for (int i = 0; i < GameLogic.BlockCountX; ++i)
            {
                for (int j = 0; j < GameLogic.BlockCountY; ++j)
                {
                    if (GameLogic.Singleton.PlayingStageData.CheckFlag(i, j, GridFlag.Jelly) || GameLogic.Singleton.PlayingStageData.CheckFlag(i, j, GridFlag.JellyDouble))
                    {
                        CapBlock pBlock = GameLogic.Singleton.GetBlock(new Position(i, j));
                        if (pBlock != null && pBlock.color < TBlockColor.EColor_Nut1 && pBlock.CurState == BlockState.Normal)
                        {
                            GlobalVars.UsingItemTarget = new Position(i, j);
                            SetTarget(GlobalVars.UsingItemTarget);
                            break;
                        }
                    }
                }
            }
        }
        
        //不是冰块关，找个可点块
        for (int i = 0; i < GameLogic.BlockCountX; ++i)
        {
            for (int j = 0; j < GameLogic.BlockCountY; ++j)
            {
                CapBlock pBlock = GameLogic.Singleton.GetBlock(new Position(i, j));
                if (pBlock != null && pBlock.color < TBlockColor.EColor_Nut1 && pBlock.CurState == BlockState.Normal)
                {
                    GlobalVars.UsingItemTarget = new Position(i, j);
                    SetTarget(GlobalVars.UsingItemTarget);
                    break;
                }
            }
        }
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
                    GlobalVars.UsingItem = PurchasedItem.None;
                    GameLogic.Singleton.ProcessTempBlocks();
                    NGUITools.PlaySound(CapsConfig.CurAudioList.PurchaseClip);
                }
                m_target.SetActive(false);
            }
        });
    }

    public void OnCancelClicked()
    {
        GlobalVars.UsingItem = PurchasedItem.None;
        HideWindow(delegate()
        {
            
        });
        if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
        {
            UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
        }
    }
}
