using UnityEngine;
using System.Collections;
using System;

public class UIPurchase : UIWindow 
{
	UILabel m_msgLabel;
	public delegate void OnPurchaseFunc();
	
	public OnPurchaseFunc OnPurchase; 
	
    public override void OnCreate()
    {
        base.OnCreate();
        AddChildComponentMouseClick("ConfirmBtn", OnConfirmClicked);
        AddChildComponentMouseClick("CancelBtn", OnCancelClicked);
		m_msgLabel = GetChildComponent<UILabel>("MessageLabel");
    }
    public override void OnShow()
    {
        base.OnShow();
        GameLogic.Singleton.PauseGame();
        if (GlobalVars.CurStageData.StepLimit > 0)
        {
            m_msgLabel.text = string.Format("You have {0} coins now,\nit will take you 1 coin to get extra 5 step,\nAre you sure about the purchasing?", GlobalVars.Coins);
        }
        else if(GlobalVars.CurStageData.TimeLimit > 0)
        {
            m_msgLabel.text = string.Format("You have {0} coins now,\nit will take you 1 coin to get extra 15 seconds,\nAre you sure about the purchasing?", GlobalVars.Coins);
        }
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            GameLogic.Singleton.ResumeGame();
        });
		OnPurchase();
    }

    private void OnCancelClicked()
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
