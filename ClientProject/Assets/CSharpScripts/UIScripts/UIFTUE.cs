using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFTUE : UIWindow
{
    AocTypewriterEffect m_dialogText;
    UISprite m_dialogBoardSprite;
	UISprite m_headSprite;
    UISprite m_backPic;

    WindowEffectFinished m_afterDialogFunc;
	
	string [] m_dialogContents;
	int m_curDialogIndex;
	
	bool m_bLock;

    public void ShowText(string head, string content, WindowEffectFinished func)
	{
		ShowWindow();
		if(head != "None")
		{
            m_headSprite.gameObject.SetActive(true);
            m_headSprite.spriteName = head;
		}
		else
		{
            m_headSprite.gameObject.SetActive(false);
		}
		
		m_dialogContents = content.Split('@');
		m_curDialogIndex = 0;
		
		m_bLock = true;
		
        m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()
        {
            if (m_curDialogIndex == m_dialogContents.Length - 1)
            {
                func();
            }
			++m_curDialogIndex;
			m_bLock = false;
        });

        m_afterDialogFunc = func;
	}
	
    public override void OnCreate()
    {
        base.OnCreate();
		m_dialogText = GetChildComponent<AocTypewriterEffect>("DialogText");
		m_headSprite = GetChildComponent<UISprite>("Head");
        m_dialogBoardSprite = GetChildComponent<UISprite>("DialogBoard");
		
		AddChildComponentMouseClick("DialogBoard", OnClick);
    }
	
	public void OnClick()
	{
		if(m_bLock)
			return;
		
		if(m_curDialogIndex < m_dialogContents.Length)
		{
			m_dialogText.Play(m_dialogContents[m_curDialogIndex], delegate()
	        {
				++m_curDialogIndex;
				if(m_curDialogIndex == m_dialogContents.Length)
				{
					m_bLock = true;
                    m_afterDialogFunc();
				}
				else
				{
					m_bLock = false;
				}
	        });	
			
			m_bLock = true;
		}
	}
}
