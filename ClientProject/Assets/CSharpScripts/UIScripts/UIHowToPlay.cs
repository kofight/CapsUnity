using UnityEngine;
using System.Collections;

public class UIHowToPlay : UIWindow 
{
    int m_curPicNum;

    int pageCount = 4;

    public override void OnCreate()
    {
        base.OnCreate();

        AddChildComponentMouseClick("CloseBtn", OnClose);
		
		AddChildComponentMouseClick("NextBtn", Next);
        AddChildComponentMouseClick("PreBtn", Pre);
    }

    public override void OnShow()
    {
        base.OnShow();
        m_curPicNum = 0;
        Refresh();
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

    void Pre()
    {
        m_curPicNum = (pageCount + m_curPicNum - 1) % pageCount;
        Refresh();
    }

    void Next()
    {
        m_curPicNum = (m_curPicNum + 1) % pageCount;
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < pageCount; ++i)
        {
            Transform help = UIToolkits.FindChild(mUIObject.transform, "Help" + (i + 1));
            if (i == m_curPicNum)
            {
                help.gameObject.SetActive(true);
            }
            else
            {
                help.gameObject.SetActive(false);
            }
        }
    }
}
