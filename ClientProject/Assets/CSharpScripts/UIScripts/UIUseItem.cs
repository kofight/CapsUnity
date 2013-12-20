using UnityEngine;
using System.Collections;
using System;

public class UIUseItem : UIWindow 
{
	UILabel m_msgLabel;
	public delegate void OnUseFunc();

    public OnUseFunc OnUse; 
	
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
        GameLogic.Singleton.HideHelp();
		//m_msgLabel.text = string.Format("You have {} coins now,\nThe item will take you 1 coin,\nAre you sure about the purchasing?", GlobalVars.Coins);
    }

    private void OnConfirmClicked()
    {
        HideWindow(delegate()
        {
            GameLogic.Singleton.ShowHelpAnim();
        });
        OnUse();
    }

    private void OnCancelClicked()
    {
        HideWindow(delegate()
        {
            GameLogic.Singleton.ShowHelpAnim();
        });
    }
}
