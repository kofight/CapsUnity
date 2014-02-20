using UnityEngine;
using System.Collections;
using System;

public class UIPurchaseNoTarget : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_costLabel;
	public delegate void OnPurchaseFunc();
	
	public OnPurchaseFunc OnPurchase; 
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_costLabel = GetChildComponent<UILabel>("CostLabel");
    }
    public override void OnShow()
    {
        base.OnShow();
        GameLogic.Singleton.PauseGame();
    }

    public void SetString(string str)
    {
        m_msgLabel.text = str;
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            GameLogic.Singleton.ResumeGame();
        });
		OnPurchase();
    }

    public void OnCancelClicked()
    {
        HideWindow(delegate()
        {
            GameLogic.Singleton.ResumeGame();
        });
        if (GameLogic.Singleton.GetGameFlow() == TGameFlow.EGameState_End)
        {
            UIWindowManager.Singleton.GetUIWindow<UIGameEnd>().ShowWindow();
        }
    }
}
