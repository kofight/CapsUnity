using UnityEngine;
using System.Collections;
using System;

public class UIMessageBox : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_waitLabel;
    UIWindow.WindowEffectFinished m_finishedFunc;
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_msgLabel = GetChildComponent<UILabel>("MessageLabel");
		AddChildComponentMouseClick("OKBtn", delegate()
		{
            if (m_finishedFunc != null)     //结束函数
            {
                m_finishedFunc();
            }
			HideWindow();	
		});
    }

    public void SetFunc(UIWindow.WindowEffectFinished func)
    {
        m_finishedFunc = func;
    }
	
	public void SetString(string str)
    {
        m_msgLabel.text = str;
    }
}
