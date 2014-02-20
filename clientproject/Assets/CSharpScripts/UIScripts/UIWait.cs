using UnityEngine;
using System.Collections;
using System;

public class UIWait : UIWindow 
{
	UILabel m_msgLabel;
	UILabel m_waitLabel;
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_msgLabel = GetChildComponent<UILabel>("IntroduceLabel");
		m_waitLabel = GetChildComponent<UILabel>("WaitLabel");
    }
    public override void OnShow()
    {
        base.OnShow();
    }

    public void SetString(string str)
    {
        m_msgLabel.text = str;
    }

    public override void OnUpdate ()
	{
		base.OnUpdate ();
		float time = Timer.GetRealTimeSinceStartUp() - Mathf.Floor(Timer.GetRealTimeSinceStartUp());
		if(time > 0.75f)
		{
			m_waitLabel.text = "...";
		}
		else if(time > 0.5f)
		{
			m_waitLabel.text = "..";
		}
		else if(time > 0.25f)
		{
			m_waitLabel.text = ".";
		}
		else
		{
			m_waitLabel.text = "";
		}
	}
}
