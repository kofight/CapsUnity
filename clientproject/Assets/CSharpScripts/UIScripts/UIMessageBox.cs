using UnityEngine;
using System.Collections;
using System;

public class UIMessageBox : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_waitLabel;
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_msgLabel = GetChildComponent<UILabel>("MessageLabel");
		AddChildComponentMouseClick("OKBtn", delegate()
		{
			HideWindow();	
		});
    }
	
	public void SetString(string str)
    {
        m_msgLabel.text = str;
    }
}
