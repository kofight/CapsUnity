using UnityEngine;
using System.Collections;

public class UISplash : UIWindowNGUI {
	
	public bool FinishProp{ get; set; }
	float m_curAlpha = 0.0f;
	float m_startAlphaTime = 0.0f;
	readonly float m_logoTime = 2.0f;
    readonly float m_waitTime = 3.0f;
	public override void OnShow ()
	{
		base.OnShow();
		m_startAlphaTime = Time.realtimeSinceStartup;
	}

    public void Close()
    {
        if (Time.realtimeSinceStartup - m_startAlphaTime < m_logoTime)
        {
            return;
        }
        HideWindow();
        UIWindowManager.Singleton.GetUIWindow<UILogin>().ShowWindow();
        CapsApplication.Singleton.HasSeenSplash = true;
    }
	
	public override void OnUpdate ()
	{
		base.OnUpdate();
		m_curAlpha = (Time.realtimeSinceStartup - m_startAlphaTime) / m_logoTime;
		if(m_curAlpha > 1.0f && uiWindowState == UIWindowStateEnum.Show)
		{
			m_curAlpha = 1.0f;
		}

        if (Time.realtimeSinceStartup - m_startAlphaTime > m_logoTime + m_waitTime)
        {
            Close();
        }

		UITexture tex = GetChildComponent<UITexture>("Texture");
		tex.alpha = m_curAlpha;

        
	}
}
