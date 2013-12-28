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

        AddChildComponentMouseClick("CloseBtn", delegate()
        {
            HideWindow(delegate()
            {
                GameLogic.Singleton.ResumeGame();
            });
            UIWindowManager.Singleton.GetUIWindow<UIMainMenu>().ShowWindow();
        });
		
		AddChildComponentMouseClick("NextBtn", Next);
    }

    public override void OnShow()
    {
        base.OnShow();
        m_curPicNum = 0;
        Texture tex = ResourceManager.Singleton.GetIconByName("help" + m_curPicNum);
        m_helpTex.mainTexture = tex;
        GameLogic.Singleton.PauseGame();
    }

    void Next()
    {
        m_curPicNum = (m_curPicNum + 1)%3;
        Texture tex = ResourceManager.Singleton.GetIconByName("help" + m_curPicNum);
        m_helpTex.mainTexture = tex;
    }
}
