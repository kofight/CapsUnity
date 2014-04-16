using UnityEngine;
using System.Collections;

public class UIHowToPlay : UIWindow 
{
    int m_curPicNum;
    UITexture m_helpTex;

    public override void OnCreate()
    {
        base.OnCreate();

        m_helpTex = GetChildComponent<UITexture>("HelpPic");

        AddChildComponentMouseClick("CloseBtn", OnClose);
		
		AddChildComponentMouseClick("NextBtn", Next);
    }

    public override void OnShow()
    {
        base.OnShow();
        m_curPicNum = 0;
        Texture tex = ResourceManager.Singleton.GetIconByName("help" + m_curPicNum);
        if (tex != null)
        {
            m_helpTex.mainTexture = tex;
			if(GameLogic.Singleton != null)
            	GameLogic.Singleton.PauseGame();
        }
    }

    public void OnClose()
    {
        HideWindow(delegate()
        {
            if (GameLogic.Singleton != null && CapsApplication.Singleton.CurStateEnum == StateEnum.Game)
            	GameLogic.Singleton.ResumeGame();
        });
        UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
    }

    void Next()
    {
        m_curPicNum = (m_curPicNum + 1)%3;
        Texture tex = ResourceManager.Singleton.GetIconByName("help" + m_curPicNum);
        m_helpTex.mainTexture = tex;
    }
}
